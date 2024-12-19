/*using Newtonsoft.Json;

namespace Mir_Utilities;

public class MirTaskHarness
{
    
    private Boolean _isRunning = false;
    
    public event EventHandler<UpdateEventArgs>? UpdateEvent;
    
    protected virtual void OnUpdateEvent(UpdateEventArgs e)
    {
        LastUpdate = e;
        EventHandler<UpdateEventArgs>? handler = UpdateEvent;
        handler?.Invoke(this, e);
        
    }
    
    public UpdateEventArgs LastUpdate { get; private set; } = new UpdateEventArgs();
    
    //Entry Points
    
    //Syncs a map between a source robot and a list of target robots
    public async Task<UpdateEventArgs> SyncMapBetweenRobots(MirRobotApi.MiRRobot source, List<MirRobotApi.MiRRobot> targets, String mapGuid )
    {
        //verify that the operation is not already running, if not set _isRunning to true
        if (_isRunning)
        {
            UpdateEventArgs args = new UpdateEventArgs();
            args.Status = UpdateEventArgs.TaskStatus.RUNNING;
            args.StatusMsg = "An Operation is already running";
            OnUpdateEvent(args);
            return LastUpdate;
        }
        _isRunning = true;
        
        
        MirRobotApi? sourceApi = await CreateApiCaller(source.Ip, source.AuthId);
        //catch if the source robot is not reachable
        if(sourceApi == null)
        {
            _isRunning = false;
            return LastUpdate;
        }
        
        MiRRobotApi sourceRobot = new MiRRobotApi(sourceApi);


        List<Task<MirRobotApi?>> targetApis = new List<Task<MirRobotApi?>>();
        foreach (var target in targets)
        {
            targetApis.Add(Task.Run(() => CreateApiCaller(target.Ip, target.AuthId)));
        }

        await Task.WhenAll(targetApis);
        List<MiRRobotApi> targetRobots = new List<MiRRobotApi>();
        //catch if any of the target robots are not reachable
        foreach (var targetApi in targetApis)
        {
            if(targetApi.Result == null)
            {
                _isRunning = false;
                return LastUpdate;
            }
            targetRobots.Add(new MiRRobotApi(targetApi.Result));
        }
        
        

        try
        {
            
            //get the map from the source robot
            Map map = await sourceRobot.CreateMap(mapGuid, OnUpdateEvent);
            
            //
            
            
            List<Task> syncTasks = new List<Task>();
            int successCount = targetRobots.Count;
            List<UpdateEventArgs.Failure> failures = new List<UpdateEventArgs.Failure>();
            
            
            foreach (var targetRobot in targetRobots)
            {
                syncTasks.Add(Task.Run(() =>
                {

                    try
                    {
                        targetRobot.ImportMap(map, true, null, OnUpdateEvent);
                    }
                    catch (Exception e)
                    {
                        UpdateEventArgs args = new UpdateEventArgs();
                        args.Status = UpdateEventArgs.TaskStatus.PARTIALFAILURE;
                        args.StatusMsg = "An Error occured while syncing map to target robot";
                        UpdateEventArgs.Failure failureObject = new UpdateEventArgs.Failure();
                        failureObject.Exception = e;
                        failureObject.Message = e.Message;
                        failureObject.StackTrace = e.StackTrace;
                        failures.Add(failureObject);
                        args.FailureObjects = new List<UpdateEventArgs.Failure>{failureObject};
                        args.Object = map;
                        successCount--;
                        OnUpdateEvent(args);
                    }
                }));
            }

            await Task.WhenAll(syncTasks);
            UpdateEventArgs finalArgs = new UpdateEventArgs();
            if(successCount == targetRobots.Count)
            {
                finalArgs.Status = UpdateEventArgs.TaskStatus.COMPLETED;
                finalArgs.StatusMsg = "Map Synced to all target robots";
                finalArgs.Object = map;
                OnUpdateEvent(finalArgs);
               
            }
            else if(successCount == 0)
            {
                finalArgs.Status = UpdateEventArgs.TaskStatus.FAILED;
                finalArgs.StatusMsg = "Failed to sync map to any target robot";
                finalArgs.FailureObjects = failures;
                finalArgs.Object = map;
                OnUpdateEvent(finalArgs);
            }
            else
            {
                finalArgs.Status = UpdateEventArgs.TaskStatus.PARTIALFAILURE;
                finalArgs.StatusMsg = "Map synced to some target robots";
                finalArgs.FailureObjects = failures;
                finalArgs.Object = map;
                OnUpdateEvent(finalArgs);
            }


        }catch(Exception e)
        {
            UpdateEventArgs args = new UpdateEventArgs();
            args.Status = UpdateEventArgs.TaskStatus.FAILED;
            args.StatusMsg = "Failed to get map from source robot";
            UpdateEventArgs.Failure failureObject = new UpdateEventArgs.Failure();
            failureObject.Exception = e;
            failureObject.Message = e.Message;
            failureObject.StackTrace = e.StackTrace;
            args.FailureObjects = new List<UpdateEventArgs.Failure>{failureObject};
            OnUpdateEvent(args);
            _isRunning = false;
            return LastUpdate;
        }

        _isRunning = false;
        return LastUpdate;

    }

    public async Task<UpdateEventArgs> ExportMapToObject(MirRobotApi.MiRRobot source, String mapGuid)
    {
        if (_isRunning)
        {
            UpdateEventArgs args = new UpdateEventArgs();
            args.Status = UpdateEventArgs.TaskStatus.RUNNING;
            args.StatusMsg = "An Operation is already running";
            OnUpdateEvent(args);
            return LastUpdate;
        }
        _isRunning = true;
        
        MirRobotApi? sourceApi = await CreateApiCaller(source.Ip, source.AuthId);
        //catch if the source robot is not reachable
        if(sourceApi == null)
        {
            _isRunning = false;
            return LastUpdate;
        }
        
        MiRRobotApi sourceRobot = new MiRRobotApi(sourceApi);
        
        
        
        _isRunning = false;
        return LastUpdate;
    }

    public async Task<UpdateEventArgs> ExportMapToString(MirRobotApi.MiRRobot source, String mapGuid)
    {
        UpdateEventArgs args = await this.ExportMapToObject( source,  mapGuid);
        args.Object = JsonConvert.SerializeObject(args.Object);
        return args;
    }
    
    public async Task<UpdateEventArgs> ImportMapFromObject(MirRobotApi.MiRRobot target, Map map)
    {

        if (_isRunning)
        {
            UpdateEventArgs args = new UpdateEventArgs();
            args.Status = UpdateEventArgs.TaskStatus.RUNNING;
            args.StatusMsg = "An Operation is already running";
            OnUpdateEvent(args);
            return LastUpdate;
        }   
        _isRunning = true;
        
        
        

        MirRobotApi? targetApi = await CreateApiCaller(target.Ip, target.AuthId);

        //catch if the source robot is not reachable
        if(targetApi == null)
        {
            _isRunning = false;
            return LastUpdate;
        }
        MiRRobotApi targetRobot = new MiRRobotApi(targetApi);
        try
        {
            targetRobot.ImportMap(map, true, null, OnUpdateEvent);
        }
        catch (Exception e)
        {
            UpdateEventArgs args = new UpdateEventArgs();
            args.Status = UpdateEventArgs.TaskStatus.FAILED;
            args.StatusMsg = "An Error occured while syncing map to target robot";
            UpdateEventArgs.Failure failureObject = new UpdateEventArgs.Failure();
            failureObject.Exception = e;
            failureObject.Message = e.Message;
            failureObject.StackTrace = e.StackTrace;
            args.FailureObjects = new List<UpdateEventArgs.Failure>{failureObject};
            args.Object = map;
            OnUpdateEvent(args);
        }
        
        
        _isRunning = false;
        return LastUpdate;
    }
    
    public async Task<UpdateEventArgs> ImportMapFromString(String mapString, MirRobotApi.MiRRobot target)
    {
        Map map = JsonConvert.DeserializeObject<Map>(mapString);
        UpdateEventArgs args = await ImportMapFromObject(target, map);
        return args;
        
    }
    
    //Helper Functions

    private async Task<MirRobotApi?> CreateApiCaller(String ip, String authId)
    {
        MirRobotApi.MiRRobot robot = new MirRobotApi.MiRRobot(ip, authId);
        MirRobotApi robotApi = new MirRobotApi(robot);
        Boolean verify = await robotApi.VerifyConnectionAsync();
        if(verify)
        {
            return robotApi;
        }
        else
        {
            UpdateEventArgs args = new UpdateEventArgs();
            args.Status = UpdateEventArgs.TaskStatus.FAILED;
            args.StatusMsg = "Failed to connect to robot";
            Dictionary<String,String> failure = new Dictionary<String, String>();
            failure.Add(ip, authId);
            args.Object = failure;
            OnUpdateEvent(args);
            return null;
        }
    }
}*/