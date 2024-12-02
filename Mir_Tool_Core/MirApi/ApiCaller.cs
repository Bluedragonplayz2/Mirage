using System.Net;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace Mir_Utilities;

public class ApiCaller
{
    //This Class handles restsharp calling, Only ever create one RestClient, this is to minimise issues with port hording.

    //Default value of Mir Robots
    private const String _protocol = "http";
    private const String _apiVersion = "v2.0.0";
    
    
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


    private async Task<RestResponse> RequestCaller(RestRequest request)
    {
        //Potential Verification of HTTPS Certificate Here
        //check is token is still valid
        if(DateTime.Compare(_tokenExpiry, DateTime.Now) < 0)
        {
            //if not get a new token
            GetMirToken();
        }
        request.AddHeader("mir-auth-token:", _authToken);
        return await  _client.ExecuteAsync(request);
    }

    private void GetMirToken()
    {
        var request = new RestRequest("auth", Method.Post);
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
                _tokenExpiry = DateTime.Parse(body.expiration_time, null,
                    System.Globalization.DateTimeStyles.RoundtripKind);
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
            throw new HttpRequestException($"error occured when executing POST:{url}, Error Code:[{response.StatusCode}], Error Message:[{response.Content}]");
        }
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
    public async void DeleteApi(String url)
    {
        var request = new RestRequest(url, Method.Delete);
        RestResponse response = await RequestCaller(request);
        if (response.StatusCode != HttpStatusCode.NoContent) 
        {
            throw new HttpRequestException($"error occured when executing DELETE:{url}, Error Code:[{response.StatusCode}], Error Message:[{response.Content}]");
        }
    }

    ~ApiCaller()
    {
        _client.Dispose();
    }

}