﻿

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
        List<Task> importingTasks = new List<Task>();
        foreach (RobotSchema.Robot target in targets)
        {
            if (!(target == source))
            { 
                Task task = Task.Run(() =>
                {
                    importSite(target, session);
                });
                importingTasks.Add(task);
            }
            
        }
        Task.WaitAll(importingTasks.ToArray());
        
        

        void importSite(RobotSchema.Robot target, byte[] session)
        {

            OnTaskUpdateEvent(new StatusObject.TaskInterimEvent(
                StatusObject.TaskInterimEvent.EventLevel.INFO,
                $"Starting site import on {target.Name}",
                target.Name
            ));
            ApiCaller targetApi = new ApiCaller(target.Ip, target.AuthId);
            try
            {
                CommonApi.VerifyRobotConnection(targetApi);
            }
            catch (Exception e)
            {
                OnTaskUpdateEvent(new StatusObject.TaskInterimEvent(
                    StatusObject.TaskInterimEvent.EventLevel.ERROR,
                    $"unable to reach {target.Name}",
                    target.Name
                ));
                taskCompleteReport.PartialFailure(
                        target.Name,
                    $"unable to reach {target.Name}",
                    e,
                    new StackTrace()
                );
                return;
            }
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
                    try
                    {
                        CommonApi.ChangeRobotState(targetApi, CommonApiSchema.RobotStatusState.State.PAUSED);
                        Thread.Sleep(100);
                        CommonApi.AdjustRobotMapAndPosition(targetApi, "mirconst-guid-0000-0001-maps00000000", nullPos);
                        Thread.Sleep(100);
                        MissionQueueApi.ClearMissionQueue(targetApi);
                        Thread.Sleep(100);
                        String s = SessionApi.DeleteSession(targetApi, sessionsSnapshot[i].Guid).Result;
                        Thread.Sleep(100);
                        CommonApi.ChangeRobotState(targetApi, CommonApiSchema.RobotStatusState.State.RESUME);
                        Thread.Sleep(100);
                    }
                    catch (Exception e)
                    {
                        OnTaskUpdateEvent(new StatusObject.TaskInterimEvent(
                            StatusObject.TaskInterimEvent.EventLevel.ERROR,
                            $"Unable to place {target.Name} into configure site",
                            target.Name
                        )); 
                        taskCompleteReport.PartialFailure(
                            target.Name,
                            $"Unable to start import site data to {target.Name}",
                            e,
                            new StackTrace()
                        );
                        return;
                    }

                    OnTaskUpdateEvent(new StatusObject.TaskInterimEvent(
                        StatusObject.TaskInterimEvent.EventLevel.INFO,
                        $"Deleted existing site {target.Name}",
                        target.Name
                    ));
                    
                }
            }
            logger.Info(session.ToString());
            SessionApiSchema.GetActiveSessionImportSnapshot sessionImport = new SessionApiSchema.GetActiveSessionImportSnapshot();
            try
            {
                if ((session == null) || (session.ToString() == null) || (session.ToString() == ""))
                {
                    OnTaskUpdateEvent(new StatusObject.TaskInterimEvent(
                        StatusObject.TaskInterimEvent.EventLevel.ERROR,
                        $"Unable to start import site data to {target.Name}",
                        target.Name
                    )); 
                    taskCompleteReport.PartialFailure(
                        target.Name,
                        $"Unable to start import site data to {target.Name}",
                        new Exception("Session is null"),
                        new StackTrace()
                    );
                    return;
                }

                sessionImport =  SessionApi.SessionImport(targetApi, session).Result;

                OnTaskUpdateEvent(new StatusObject.TaskInterimEvent(
                    StatusObject.TaskInterimEvent.EventLevel.INFO,
                    $"Importing:0",
                    target.Name
                ));
                Thread.Sleep(1000);
            }
            catch (Exception e)
            {
                OnTaskUpdateEvent(new StatusObject.TaskInterimEvent(
                    StatusObject.TaskInterimEvent.EventLevel.ERROR,
                    $"Unable to start import site data to {target.Name}",
                    target.Name
                )); 
                taskCompleteReport.PartialFailure(
                    target.Name,
                    $"Unable to start import site data to {target.Name}",
                    e,
                    new StackTrace()
                );
                return;
            }

            try
            {
                while (sessionImport.Status == 1)
                {
                    sessionImport = SessionApi.GetActiveSessionImportSnapshot(targetApi).Result;
                    if (sessionImport.SessionsTotal == 0)
                    {
                        sessionImport.SessionsTotal = 1;
                        sessionImport.SessionsImported = 1;
                    }
                    OnTaskUpdateEvent(new StatusObject.TaskInterimEvent(
                        StatusObject.TaskInterimEvent.EventLevel.INFO,
                        $"Importing:{(sessionImport.SessionsImported * 100 )/sessionImport.SessionsTotal}",
                        target.Name
                    ));
                    Thread.Sleep(1000);
                }

                if (sessionImport.Status == 2)
                {
                    OnTaskUpdateEvent(new StatusObject.TaskInterimEvent(
                        StatusObject.TaskInterimEvent.EventLevel.ERROR,
                        sessionImport.ErrorMessage,
                        target.Name
                    )); 
                    taskCompleteReport.PartialFailure(
                        target.Name,
                        sessionImport.ErrorMessage,
                        new Exception("Failed to import site"),
                        new StackTrace()
                    );
                }
                else
                {
                    OnTaskUpdateEvent(new StatusObject.TaskInterimEvent(
                        StatusObject.TaskInterimEvent.EventLevel.INFO,
                        $"Imported site data to {target.Name}",
                        target.Name
                    ));
                    if (!status.Equals(new CommonApiSchema.RobotStatus()))
                    {
                        CommonApi.AdjustRobotMapAndPosition(targetApi, status.MapId,status.RobotPosition);
                    }
                }

            }
            catch (Exception e)
            {
                OnTaskUpdateEvent(new StatusObject.TaskInterimEvent(
                    StatusObject.TaskInterimEvent.EventLevel.ERROR,
                    $"Unable to get import status from {target.Name}",
                    target.Name
                )); 
                taskCompleteReport.PartialFailure(
                    target.Name,
                    $"Unable to get import status from {target.Name}",
                    e,
                    new StackTrace()
                );
            }

            
        }
        return taskCompleteReport;
    }

    protected virtual void OnTaskUpdateEvent(StatusObject.TaskInterimEvent e)
    {
        TaskUpdateEvent?.Invoke(this, e);
    }

}