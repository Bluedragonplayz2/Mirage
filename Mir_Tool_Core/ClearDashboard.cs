using Mir_Utilities.Common;
using Mir_Utilities.MirApi;

namespace Mir_Utilities;

public class ClearDashboard
{
    private static readonly log4net.ILog logger =
        log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public static void ClearDashboardFromRobot(RobotSchema.Robot robot)
    {
        logger.Info("start dash");
        ApiCaller apiCaller = new ApiCaller(robot.Ip, robot.AuthId);
        logger.Info("finish api");
        //Get current user id to use in the request
        
        String id = apiCaller.GetApi("users/me").Result.guid;
        logger.Info(id);
        
        //Get all dashboards for the user
        dynamic dashboards = apiCaller.GetApi("dashboards").Result;
        foreach (dynamic dashboard in dashboards )
        {
            dynamic dashboardSnapshot =  apiCaller.GetApi("dashboards/" + dashboard.guid).Result;
            string dashboardCreatorId = dashboardSnapshot.created_by_id;
            logger.Info(dashboardCreatorId);

            if (dashboardCreatorId == id)
            {
                apiCaller.DeleteApi("dashboards/"+dashboard.guid);
            }
        }
    }
}