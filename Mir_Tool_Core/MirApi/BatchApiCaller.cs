using System.Net;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace Mir_Utilities.MirApi;

public class BatchApiCaller: ApiCaller
{
    //Default value of Mir Robots
    private const String _protocol = "http";
    private const String _apiVersion = "v2.0.0";
    
    private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    
    private readonly RestClient _client;
    private DateTime _tokenExpiry = DateTime.Now;
    private String _authToken = "";
    private readonly String _authId;

    
    public BatchApiCaller(string robotIp, string authId) : base(robotIp, authId)
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
        try
        {
            CreateMiRToken();
        }
        catch (Exception ex)
        {
            LOGGER.Error("Failed to generate token for the first time", ex);
        }
    }
    private async Task<RestResponse> RequestCaller(RestRequest request)
    {
        //Potential Verification of HTTPS Certificate Here
        //check is token is still valid
        if(DateTime.Compare(_tokenExpiry, DateTime.Now) < 0)
        {
            //if not get a new token
            CreateMiRToken();
        }
        request.AddHeader("mir-auth-token", _authToken);
        return await  _client.ExecuteAsync(request);
    }

    public  async Task<dynamic> GetApi(string url)
    {
        return null;
    }

    public async Task<dynamic> PostApi(string url, Object content)
    {
        return null;
    }


}