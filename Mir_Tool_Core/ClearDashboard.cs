using Mir_Utilities.Common;
using Mir_Utilities.MirApi;

namespace Mir_Utilities;

public class ClearDashboard
{
    public static void ClearDashboardFromRobot(RobotSchema.Robot robot)
    {
        ApiCaller apiCaller = new ApiCaller(robot.Ip, robot.AuthId);
        
        //Get current user id to use in the request
        
        String id = apiCaller.GetApi("users/me").Result.guid;
        
        //Get all dashboards for the user
        dynamic dashboards = apiCaller.GetApi("dashboards").Result;
        foreach (dynamic dashboard in dashboards )
        {
            dynamic dashboardSnapshot =  apiCaller.GetApi("dashboards/" + dashboard.guid).Result;
            string dashboardCreatorId = dashboardSnapshot.created_by_id;

            if (dashboardCreatorId == id)
            {
                apiCaller.DeleteApi("dashboards/"+dashboard.guid);
            }
        }
    }
}