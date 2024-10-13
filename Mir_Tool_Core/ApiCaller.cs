using System.Net;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace Mir_Utilities;

public class ApiCaller
{
    //This Class handles restsharp calling, Only ever create one RestClient, this is to minimise issues with port hording.

    public ApiCaller(String robotIp, String authId)
    {
        String url = "http://" + robotIp+"/api/v2.0.0/";
        Dictionary<String, String> headers = new Dictionary<String, String>
        {
            {"Authorization",authId},
            {"Accept-Language", "en_US"},
        };
        _client = new RestClient(url,
            configureSerialization: s => s.UseNewtonsoftJson());
        _client.AddDefaultHeaders(headers);
    }
    private RestClient _client;


    public async Task<dynamic> GetApi(String url)
    {
        var request = new RestRequest(url, Method.Get);
        
        RestResponse response = await _client.ExecuteAsync(request);
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
        RestResponse response = await _client.ExecuteAsync(request);
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
        RestResponse response = await _client.ExecuteAsync(request);
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
        RestResponse response = await _client.ExecuteAsync(request);
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