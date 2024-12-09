namespace Mir_Utilities;

public class MissionApiSchma
{
    public struct GetMissionSnapshot
    {
        public String Name;
        public String Guid;
    }
    public struct GetMissionByGuidSnapshot
    {
        public String Guid;
        public String Name;
        public String Description;
        public String? SessionId;
        public String? GroupId;
        public Boolean Hidden;
        public Boolean IsTemplate;
        public String CreatedById;
        public Boolean Valid;
        public Boolean HasUserParameters;
    }
}