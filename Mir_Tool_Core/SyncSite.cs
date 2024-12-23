

using System.Diagnostics;
using Mir_Utilities.Common;

namespace Mir_Utilities;

public class SyncSite
{
    public event EventHandler<StatusObject.TaskInterimEvent> TaskUpdateEvent;
    private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    public StatusObject.TaskCompleteReport SyncSiteData(RobotSchema.Robot source, List<RobotSchema.Robot> targets, string siteName)
    {
        StatusObject.TaskCompleteReport taskCompleteReport = new StatusObject.TaskCompleteReport(StatusObject.TaskCompleteReport.TaskStatus.COMPLETED);
        //export site data
        ApiCaller sourceApi = new ApiCaller(source.Ip, source.AuthId);
        List<SessionApiSchema.GetSessionSnapshot>? sessionsSnapshot = SessionApi.GetSessionSnapshot(sourceApi).Result;
        if (sessionsSnapshot == null)
        {
            logger.Fatal("failed to get any session from the source robot or the source robot does not have any site to begin with, is this a bug?");
            taskCompleteReport.Fatal(
                "failed to get any session from the source robot or the source robot does not have any site to begin with, is this a bug?",
                source.Name,
                "Failed to get sessions from source",
                new Exception("Failed to get sessions from source"),
                new StackTrace()
                );
            return taskCompleteReport;
        }
        int i = sessionsSnapshot.FindIndex (s => s.Name == siteName);
        if (i == -1)
        {
            logger.Fatal("The session name provided is not found in the source robot.");
            taskCompleteReport.Fatal(
                "The session name provided is not found in the source robot.",
                source.Name,
                "Failed to get sessions from source",
                new Exception("Failed to get sessions from source"),
                new StackTrace()
            );
            return taskCompleteReport;
        }
        byte[] session = SessionApi.SessionExport(sourceApi, sessionsSnapshot[i].Guid).Result;
        OnTaskUpdateEvent(new StatusObject.TaskInterimEvent(
            StatusObject.TaskInterimEvent.EventLevel.INFO,
            "Exported site data from source robot",
            source.Name
        ));
        //import site data
        foreach (RobotSchema.Robot target in targets)
        {
            Task task = Task.Run(() =>
            {
                importSite(target, session);
            });
        }

        void importSite(RobotSchema.Robot target, byte[] session)
        {
            ApiCaller targetApi = new ApiCaller(target.Ip, target.AuthId);
            List<SessionApiSchema.GetSessionSnapshot>? sessionsSnapshot = SessionApi.GetSessionSnapshot(targetApi).Result;
            CommonApiSchema.RobotStatus status = new CommonApiSchema.RobotStatus();
            if (sessionsSnapshot != null)
            {
                int i = sessionsSnapshot.FindIndex (s => s.Name == siteName);
                if (i != -1)
                {
                    status = CommonApi.GetRobotStatus(targetApi).Result;
                    if (false)
                    {
                        //Todo: Safety checks 
                    }
                    CommonApiSchema.RobotStatus.Position nullPos = new CommonApiSchema.RobotStatus.Position();
                    nullPos.PosX = 0;
                    nullPos.PosY = 0;
                    nullPos.Orientation = 0;

                    CommonApi.AdjustRobotMapAndPosition(targetApi, "mirconst-guid-0000-0001-maps00000000",nullPos);
                    SessionApi.DeleteSession(targetApi, sessionsSnapshot[i].Guid);
                    //Todo: update this to the new robot object
                    MirRobotApi.MiRRobot legacyRobot = new MirRobotApi.MiRRobot(target.Ip, target.AuthId);
                    ClearFootprint.ClearFootprintFromRobot(legacyRobot);
                    Thread.Sleep(5000);
                }
            }


            
            SessionApi.SessionImport(targetApi, session);
            
            
            
            /*
            if (status.Equals(new CommonApiSchema.RobotStatus()))
            {
                CommonApi.AdjustRobotMapAndPosition(targetApi, status.MapId,status.RobotPosition);
            }*/
            
        }

        return taskCompleteReport;
    }

    protected virtual void OnTaskUpdateEvent(StatusObject.TaskInterimEvent e)
    {
        TaskUpdateEvent?.Invoke(this, e);
    }

}