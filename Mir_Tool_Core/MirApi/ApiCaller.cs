using System.Globalization;
using System.Net;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace Mir_Utilities;

public class ApiCaller
{

    //Default value of Mir Robots
    private const String _protocol = "http";
    private const String _apiVersion = "v2.0.0";
    
    private bool _BatchMode = false;
    private List<TaskCompletionSource<RestResponse>> _individualCompletionSources = new List<TaskCompletionSource<RestResponse>>();

    private List<RestRequest> _batchRequests = new List<RestRequest>();
    
    public ApiCaller(String robotIp, String authId)
    {
        String url = _protocol + "://" + robotIp+ "/api/" + _apiVersion + "/";
        Dictionary<String, String> headers = new Dictionary<String, String>
        {
            {"Accept-Language", "en_US"},
            {"Accept", "application/json"},
        };
        
        _client = new RestClient(url,
            configureSerialization: s => s.UseNewtonsoftJson());
        _client.AddDefaultHeaders(headers);
        _authId = authId;
        //Generating First Token
        GetMirToken();
    }
    private readonly RestClient _client;
    private DateTime _tokenExpiry = DateTime.Now;
    private String _authToken = "";
    private readonly String _authId;
    
    public void SetBatchMode(bool batchMode)
    {
        _BatchMode = batchMode;
        _batchRequests.Clear();
        _individualCompletionSources.Clear(); 
    }


    private async Task<RestResponse> RequestCaller(RestRequest request)
    {
        //Potential Verification of HTTPS Certificate Here
        //check is token is still valid
        if(DateTime.Compare(_tokenExpiry, DateTime.Now) < 0)
        {
            //if not get a new token
            GetMirToken();
        }
        request.AddHeader("mir-auth-token", _authToken);
        return await  _client.ExecuteAsync(request);
    }

    private void GetMirToken()
    {
        var request = new RestRequest("users/auth", Method.Post);
        request.AddHeader("Authorization", _authId);
        RestResponse response = _client.Execute(request);
        if (response.IsSuccessful) 
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new FileNotFoundException("error occured when executing POST:auth");
            }
            else if (response.Content == null)
            {
                throw new Exception("error occured when executing POST:auth, no content returned");
            }
            else
            {
                dynamic body = JsonConvert.DeserializeObject(response.Content);
                _authToken = body.token;
                DateTime expiry = body.expiration_time;
                _tokenExpiry = expiry;
            }
        }
        else
        {
            throw new HttpRequestException($"error occured when executing POST:auth, Error Code:[{response.StatusCode}], Error Message:[{response.Content}]");
        }
    }

    public async Task<dynamic> GetApi(String url)
    {
        var request = new RestRequest(url);
        
        RestResponse response = await RequestCaller(request);
        if (response.IsSuccessful) 
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new FileNotFoundException($"error occured when executing GET:{url}");
            }
            else if(response.Content == null)
            {
                return true;
            }
            else
            {   
                dynamic? body = JsonConvert.DeserializeObject(response.Content);
                if(body == null){ return true; }
                return body;
            }
        }
        else
        {
            throw new HttpRequestException($"error occured when executing GET:{url}, Request address{_client.BuildUri(request)} ,  Error Code:[{response.StatusCode}], Error Message:[{response.Content}]");
        }

       
    }

    public async Task<dynamic> PostApi(String url, Object content)
    {
        var request = new RestRequest(url, Method.Post);
        request.AddJsonBody(content);
        RestResponse response = await RequestCaller(request);
        if (response.IsSuccessful)
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new FileNotFoundException($"error occured when executing POST:{url}");
            }
            if (response.Content == null)
            {
                return true;
            }

            dynamic? body = JsonConvert.DeserializeObject(response.Content);
            if (body == null)
            {
                return true;
            }

            return body;
        }

        throw new HttpRequestException($"error occured when executing POST:{url}, Error Code:[{response.StatusCode}], Error Message:[{response.Content}]");

    }

    public async Task<dynamic> PutApi(String url, Object content)
    {
        var request = new RestRequest(url, Method.Put);
        request.AddJsonBody(content);
        RestResponse response = await RequestCaller(request);
        if (response.IsSuccessful) 
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new FileNotFoundException($"error occured when executing PUT:{url}");
            }
            else if(response.Content == null)
            {
                return true;
            }
            else
            {
                dynamic? body = JsonConvert.DeserializeObject(response.Content);
                if(body == null){ return true; }
                return body;
            }
        }
        else
        {
            throw new HttpRequestException($"error occured when executing PUT:{url}, Error Code:[{response.StatusCode}], Error Message:[{response.Content}]");
        }
    }
    public async Task<RestResponse> DeleteApi(String url)
    {
        var request = new RestRequest(url, Method.Delete);
        RestResponse response = await RequestCaller(request);
        if (response.StatusCode != HttpStatusCode.NoContent) 
        {
            throw new HttpRequestException($"error occured when executing DELETE:{url}, Error Code:[{response.StatusCode}], Error Message:[{response.Content}]");
        }

        return response;
    }
        public async Task<List<ApiResponse>> ExecuteBatchAsync()
    {
        if (!_BatchMode)
        {
            throw new InvalidOperationException("Batch mode is not enabled. Initialize batch mode using InitBatchMode().");
        }

        var batchPayload = new List<object>();

        foreach (var request in _batchRequests)
        {
            var bodyParameter = request.Parameters.FirstOrDefault(p => p.Type == ParameterType.RequestBody);

            var requestPayload = new object();
            
            if (bodyParameter != null)
            {
                 requestPayload = new
                {
                    method = request.Method.ToString(),
                    url = request.Resource,
                    body = bodyParameter.Value
                };
            }
            else
            {
                 requestPayload = new
                {
                    method = request.Method.ToString(),
                    url = request.Resource
                };
            }

            batchPayload.Add(requestPayload);
        }

        var batchRequest = new RestRequest("/batch/", Method.Post)
            .AddJsonBody(batchPayload);

        _batchRequests.Clear(); // Clear the batch requests after executing

        var response = await _client.ExecuteAsync(batchRequest);

        var parsedResponses = ParseBatchResponse(response.Content);

        // Assign individual responses to their respective tasks
        for (int i = 0; i < parsedResponses.Count && i < _individualCompletionSources.Count; i++)
        {
            var taskSource = _individualCompletionSources[i];
            var individualResponse = new RestResponse
            {
                StatusCode = (System.Net.HttpStatusCode)parsedResponses[i].Status,
                Content = JsonConvert.SerializeObject(parsedResponses[i].Body),
            };
            taskSource.SetResult(individualResponse);
        }

        _individualCompletionSources.Clear();

        return parsedResponses;
    }

    // Method to parse batch response content
    private List<ApiResponse> ParseBatchResponse(string jsonResponse)
    {
        try
        {
            return JsonConvert.DeserializeObject<List<ApiResponse>>(jsonResponse);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Failed to parse batch response.", ex);
        }
    }
    
    public async Task<dynamic> GetRawApi(String url)
    {
        var request = new RestRequest(url);
        
        RestResponse response = await RequestCaller(request);
        if (response.IsSuccessful) 
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new FileNotFoundException($"error occured when executing GET:{url}");
            }
            else if(response.Content == null)
            {
                return true;
            }
            else
            {   
                byte[] body = response.RawBytes;
                if(body == null){ return true; }
                return body;
            }
        }
        else
        {
            throw new HttpRequestException($"error occured when executing GET:{url}, Request address{_client.BuildUri(request)} ,  Error Code:[{response.StatusCode}], Error Message:[{response.Content}]");
        }

       
    }
    public async Task<dynamic> PostFileApi(String url,string paramName , byte[] content, string fileName)
    {
        var request = new RestRequest(url, Method.Post);
        request.AddFile(paramName, content, fileName, ContentType.Binary);
        RestResponse response = await RequestCaller(request);
        if (response.IsSuccessful)
        {

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new FileNotFoundException($"error occured when executing POST:{url}");
            }
            if (response.Content == null)
            {
                return true;
            }

            dynamic? body = JsonConvert.DeserializeObject(response.Content);
            if (body == null)
            {
                return true;
            }

            return body;
        }


        throw new HttpRequestException($"error occured when executing POST:{url}, Error Code:[{response.StatusCode}], Error Message:[{response.Content}]");

    }
    public async Task<dynamic> PostRawApi(String url, Object content,string paramName , byte[] byteContent)
    {
        var request = new RestRequest(url, Method.Post);
        request.AddJsonBody(content);
        request.AddParameter(paramName, byteContent, ParameterType.RequestBody);
        RestResponse response = await RequestCaller(request);
        if (response.IsSuccessful)
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new FileNotFoundException($"error occured when executing POST:{url}");
            }
            if (response.Content == null)
            {
                return true;
            }

            dynamic? body = JsonConvert.DeserializeObject(response.Content);
            if (body == null)
            {
                return true;
            }

            return body;
        }

        throw new HttpRequestException($"error occured when executing POST:{url}, Error Code:[{response.StatusCode}], Error Message:[{response.Content}]");

    }

    public async Task<dynamic> GetTimedApi(String url)
    {
        var request = new RestRequest(url);
        request.Timeout = TimeSpan.FromSeconds(1);
        RestResponse response = await RequestCaller(request);
        if (response.IsSuccessful) 
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new FileNotFoundException($"error occured when executing GET:{url}");
            }
            else if(response.Content == null)
            {
                return true;
            }
            else
            {   
                dynamic? body = JsonConvert.DeserializeObject(response.Content);
                if(body == null){ return true; }
                return body;
            }
        }
        else
        {
            throw new HttpRequestException($"error occured when executing GET:{url}, Request address{_client.BuildUri(request)} ,  Error Code:[{response.StatusCode}], Error Message:[{response.Content}]");
        }

       
    }

    


// Models for deserializing response


    ~ApiCaller()
    {
        _client.Dispose();
    }

}
public class ApiResponse
{
    public int Status { get; set; }
    public object Body { get; set; }
    public Dictionary<string, string> Headers { get; set; }
}