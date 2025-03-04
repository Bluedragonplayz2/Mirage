using RestSharp;

namespace Mir_Utilities;

public class MissionQueueApi
{
    public static void ClearMissionQueue(ApiCaller caller)
    {
        RestResponse r = caller.DeleteApi("mission_queue").Result;
    }
}