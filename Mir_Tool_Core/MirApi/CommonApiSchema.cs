namespace Mir_Utilities;

public class CommonApiSchema
{
    public struct RobotStatus
    {
        public struct Position
        {
            public float PosX;
            public float PosY;
            public float Orientation;
        }

        public struct Velocity
        {
            public float Linear;
            public float Angular;
        }

        public Velocity RobotVelocity;
        public Position RobotPosition;
        public int BatteryTimeRemaining;
        public float BatteryPercentage;
        public string MissionQueueId;
        public string RobotName;
        public int SerialNumber;
        public string RobotModel;
        public int StateId;
        public int ModeId;
        public string MapId;
        public string SessionId;
    }
    
}