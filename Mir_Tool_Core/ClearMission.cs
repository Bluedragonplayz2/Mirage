using Mir_Utilities.Common;
using Mir_Utilities.MirApi;

namespace Mir_Utilities;

public class ClearMission
{
    public static void ClearMissionFromRobot(RobotSchema.Robot robot)
    {
        ApiCaller apiCaller = new ApiCaller(robot.Ip, robot.AuthId);
        
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
                apiCaller.DeleteApi("missions/"+mission.guid);
            }
        }
    }
}