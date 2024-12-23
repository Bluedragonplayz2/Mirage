namespace Mir_Utilities;

public class CommonApi
{
    public struct RobotPos
    {
        public float X;
        public float Y;
        public float Orientation;
        public string MapId;
    }
    public static async Task<CommonApiSchema.RobotStatus> GetRobotStatus(ApiCaller caller)
    {
        dynamic status = await caller.GetApi("status");
        CommonApiSchema.RobotStatus robotStatus = new CommonApiSchema.RobotStatus();
        CommonApiSchema.RobotStatus.Position position = new CommonApiSchema.RobotStatus.Position();
        position.PosX = status.position.x!;
        position.PosY = status.position.y!;
        position.Orientation = status.position.orientation!;
        robotStatus.RobotPosition = position;
        CommonApiSchema.RobotStatus.Velocity velocity = new CommonApiSchema.RobotStatus.Velocity();
        velocity.Linear = status.velocity.linear!;
        velocity.Angular = status.velocity.angular!;
        robotStatus.RobotVelocity = velocity;
        robotStatus.BatteryPercentage = status.battery_percentage!;
        robotStatus.BatteryTimeRemaining = status.battery_time_remaining!;
        robotStatus.MissionQueueId = status.mission_queue_id!;
        robotStatus.RobotName = status.robot_name!;
        robotStatus.RobotModel = status.robot_model!;
        robotStatus.SerialNumber = status.serial_number!;
        robotStatus.StateId = status.state_id!;
        robotStatus.ModeId = status.mode_id!;
        robotStatus.MapId = status.map_id!;
        robotStatus.SessionId = status.session_id!;
        return robotStatus;
    }
    
    
    
    public static void AdjustRobotMapAndPosition(ApiCaller caller, String mapId, CommonApiSchema.RobotStatus.Position newPos)
    {
        dynamic status = new
        {
            map_id = mapId,
            position = new
            {
                x = newPos.PosX,
                y = newPos.PosY,
                orientation = newPos.Orientation
            }
        };


    }
}