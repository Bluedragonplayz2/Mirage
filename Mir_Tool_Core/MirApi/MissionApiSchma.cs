namespace Mir_Utilities.MirApi;

public class MissionApiSchma
{
    public struct GetMissionSnapshot
    {
        public string Name;
        public string Guid;
    }
    public struct GetMissionByGuidSnapshot
    {
        public string Guid;
        public string Name;
        public string Description;
        public string? SessionId;
        public string? GroupId;
        public bool Hidden;
        public bool IsTemplate;
        public string CreatedById;
        public bool Valid;
        public bool HasUserParameters;
    }

    public struct GetActionByMission
    {
        public string Guid;
        public string Name;
        public string MissionId;
    }
    
    public struct GetActionByGuid
    {
        public string Guid;
        public string ActionType;
        public string MissionId;
        public int Priority;
        public string ScopeReference;
        public string CreatedById;

        public struct Parameter
        {
            public string Id;
            public string Value;
            public string InputName;
            public string Guid;
        }
        public List<Parameter> ParametersList;
    }
    
    
}