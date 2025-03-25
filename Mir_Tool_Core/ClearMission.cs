using Mir_Utilities.Common;
using Mir_Utilities.MirApi;

namespace Mir_Utilities;

public class ClearMission
{
    public static void ClearMissionFromRobot(RobotSchema.Robot robot, string siteName)
    {

        
        ApiCaller apiCaller = new ApiCaller(robot.Ip, robot.AuthId);
        
        string siteGuid = "";
        if (siteName != "")
        {
            List<SessionApiSchema.GetSessionSnapshot> sessionSnapshots = SessionApi.GetSessionSnapshot(apiCaller).Result ?? new List<SessionApiSchema.GetSessionSnapshot>();
            sessionSnapshots.ForEach(session =>
            {
                if (session.Name == siteName)
                {
                    siteGuid = session.Guid;
                }
            });
        }
        
        //Get current user id to use in the request
        
        String id = apiCaller.GetApi("users/me").Result.guid;
        
        //Get all footprints for the user
        dynamic missions = apiCaller.GetApi("missions").Result;
        foreach (dynamic mission in missions )
        {
            dynamic missionSnapshot =  apiCaller.GetApi("missions/" + mission.guid).Result;
            string missionCreatorId = missionSnapshot.created_by_id;

            if (missionCreatorId == id)
            {
                if (siteGuid == "" || missionSnapshot.session_id == siteGuid)
                {
                    apiCaller.DeleteApi("missions/"+mission.guid);
                }
            }
        }
    }
}