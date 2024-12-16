using log4net;
using log4net.Repository.Hierarchy;

namespace Mir_Utilities;

public class SyncSite
{
    private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    public static void SyncSiteData(RobotSchema.Robot source, List<RobotSchema.Robot> targets, string siteName)
    {
        //export site data
        ApiCaller sourceApi = new ApiCaller(source.Ip, source.AuthId);
        List<SessionApiSchema.GetSessionSnapshot>? sessionsSnapshot = SessionApi.GetSessionSnapshot(sourceApi).Result;
        if (sessionsSnapshot == null)
        {
            logger.Fatal("failed to get any session from the source robot or the source robot does not have any site to begin with, is this a bug? ");
            throw new Exception("Failed to get sessions from source");
        }
        int i = sessionsSnapshot.FindIndex (s => s.Name == siteName);
        if (i == -1)
        {
            logger.Fatal("The session name provided is not found in the source robot.");
            throw new Exception("Failed to get sessions from source");
        }
        string session = SessionApi.SessionExport(sourceApi, sessionsSnapshot[i].Guid).Result;

        //import site data
        foreach (RobotSchema.Robot target in targets)
        {
            Task task = Task.Run(() =>
            {
                importSite(target, session);
            });
        }

        void importSite(RobotSchema.Robot target, String session)
        {
            ApiCaller targetApi = new ApiCaller(target.Ip, target.AuthId);
            List<SessionApiSchema.GetSessionSnapshot>? sessionsSnapshot = SessionApi.GetSessionSnapshot(targetApi).Result;
            if (sessionsSnapshot != null)
            {
                int i = sessionsSnapshot.FindIndex (s => s.Name == siteName);
                if (i != -1)
                {
                    
                    SessionApi.DeleteSession(targetApi, sessionsSnapshot[i].Guid);
                    //Todo: update this to the new robot object
                    MirRobotApi.MiRRobot legacyRobot = new MirRobotApi.MiRRobot(target.Ip, target.AuthId);
                    ClearFootprint.ClearFootprintFromRobot(legacyRobot);
                    Thread.Sleep(5000);
                }
            }
            SessionApi.SessionImport(targetApi, session);
        }
    }

}