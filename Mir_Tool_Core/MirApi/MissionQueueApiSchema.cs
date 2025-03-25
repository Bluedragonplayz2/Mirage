namespace Mir_Utilities.MirApi;

public class MissionQueueApiSchema
{
    public struct GetMissionQueue
    {
        public int Id;
        public string State;
    }
    public struct GetMissionQueueById
    {
        public int Id;
        public string MissionId;
        public string Mission;
        public string State;
        public string Message;
        public DateTime Ordered;
        public DateTime? Started;
        public DateTime? Finished;
        public int Priority;
        public string Actions;
        public string CreatedById;
        public int ControlState;
        public string ControlPosId;
        public string Description;
    }
}