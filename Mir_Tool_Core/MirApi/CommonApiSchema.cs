namespace Mir_Utilities.MirApi;
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
    public struct RobotStatusState
    {
        public enum State
        {
            //Todo: Populate Other States
            READY =3,
            PAUSED =4,
            EXECUTING =5,
            ESTOP = 10,
            MANUALCONTROL = 11,
            ERROR = 12,
        }
        public State RobotMissionQueueState;
    }
}