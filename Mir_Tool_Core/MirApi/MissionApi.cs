namespace Mir_Utilities.MirApi;
public class MissionApi
{
    public static async Task<List<MissionApiSchma.GetMissionSnapshot>?> GetMissions(ApiCaller caller)
    {
        dynamic missionList = await caller.GetApi($"missions");
        if (missionList.Count == 0)
        {
            return null;
        }

        List<MissionApiSchma.GetMissionSnapshot> missionListSnapshot = new List<MissionApiSchma.GetMissionSnapshot>();

        foreach (var mission in missionList)
        {
            MissionApiSchma.GetMissionSnapshot missionSnapshot = new MissionApiSchma.GetMissionSnapshot();
            missionSnapshot.Name = mission.name!;
            missionSnapshot.Guid = mission.guid!;
            missionListSnapshot.Add(missionSnapshot);
        }
        return missionListSnapshot;
    }
    public static async Task<MissionApiSchma.GetMissionByGuidSnapshot> GetMissionByGuid(ApiCaller caller, string guid)
    {
        dynamic mission = await caller.GetApi($"missions/{guid}");
        MissionApiSchma.GetMissionByGuidSnapshot missionSnapshot = new MissionApiSchma.GetMissionByGuidSnapshot();
        missionSnapshot.Name = mission.name!;
        missionSnapshot.Guid = mission.guid!;
        missionSnapshot.Description = mission.description!;
        missionSnapshot.GroupId = mission.group_id!;
        missionSnapshot.Hidden = mission.hidden!;
        missionSnapshot.Valid = mission.valid!;
        missionSnapshot.IsTemplate = mission.is_template!;
        missionSnapshot.SessionId = mission.session_id!;
        missionSnapshot.CreatedById = mission.created_by_id!;
        missionSnapshot.HasUserParameters = mission.has_user_parameters!;
        return missionSnapshot;
    }
    public static async Task<string> PostMission(ApiCaller caller, string guid, string name, string description, string sessionId, string groupId, bool hidden)
    {
        dynamic mission = new
        {
            guid,
            name,
            description,
            session_id = sessionId,
            group_id = groupId,
            hidden,

        };
        dynamic response = await caller.PostApi("missions", mission);
        return response.guid!;
    }
    public static async Task<string> PutMission(ApiCaller caller, string guid, string name, string description, string sessionId, string groupId, bool hidden)
    {
        dynamic mission = new
        {
            name,
            description,
            session_id = sessionId,
            group_id = groupId,
            hidden
        };
        dynamic response = await caller.PutApi($"missions/{guid}", mission);
        return response.guid!;
    }
    public void DeleteMission(ApiCaller caller, string guid)
    {
        caller.DeleteApi($"missions/{guid}");
    }
    public static async Task<List<MissionApiSchma.GetActionByMission>?> GetActionsByMission(ApiCaller caller, string missionGuid)
    {
        dynamic actionList = await caller.GetApi($"missions/{missionGuid}/actions");
        if (actionList.Count == 0)
        {
            return null;
        }

        List<MissionApiSchma.GetActionByMission> actionListSnapshot = new List<MissionApiSchma.GetActionByMission>();

        foreach (var action in actionList)
        {
            MissionApiSchma.GetActionByMission actionSnapshot = new MissionApiSchma.GetActionByMission();
            actionSnapshot.Name = action.name!;
            actionSnapshot.Guid = action.guid!;
            actionSnapshot.MissionId = action.mission_id!;
            actionListSnapshot.Add(actionSnapshot);
        }
        return actionListSnapshot;
    }
    public static async Task<MissionApiSchma.GetActionByGuid> GetActionByGuid(ApiCaller caller, string missionGuid, string actionGuid)
    {
        dynamic action = await caller.GetApi($"missions/{missionGuid}/actions/{actionGuid}");
        MissionApiSchma.GetActionByGuid actionSnapshot = new MissionApiSchma.GetActionByGuid();
        actionSnapshot.Guid = action.guid!;
        actionSnapshot.MissionId = action.mission_id!;
        actionSnapshot.ActionType = action.action_type!;
        actionSnapshot.CreatedById = action.created_by_id!;
        actionSnapshot.Priority = action.priority!;
        actionSnapshot.ScopeReference = action.scope_reference!;
        List<MissionApiSchma.GetActionByGuid.Parameter> parametersList = new List<MissionApiSchma.GetActionByGuid.Parameter>();
        foreach (var parameter in action.parameters)
        {
            MissionApiSchma.GetActionByGuid.Parameter parameterSnapshot = new MissionApiSchma.GetActionByGuid.Parameter();
            parameterSnapshot.Value = parameter.value!;
            parameterSnapshot.Guid = parameter.guid!;
            parameterSnapshot.Id = parameter.id!;
            parameterSnapshot.InputName = parameter.input_name!;
            parametersList.Add(parameterSnapshot);
        }
        actionSnapshot.ParametersList = parametersList;
        return actionSnapshot;
    }
    public static async Task<string> PostAction(ApiCaller caller, string guid, string name, string actionType, string missionId, int priority, string scopeReference, List<MissionApiSchma.GetActionByGuid.Parameter> parameterList)
    {
        List<dynamic> parameters = [];
        foreach (var parameter in parameterList)
        {
            dynamic actionParameter = new
            {
                guid = parameter.Guid,
                value = parameter.Value,
                id = parameter.Id,
                input_name = parameter.InputName
            };
            parameters.Add(actionParameter);
        }
        dynamic action = new
        {
            guid,
            name,
            action_type = actionType,
            mission_id = missionId,
            priority,
            scope_reference = scopeReference,
            parameters
        };
        dynamic response = await caller.PostApi($"missions/{missionId}/actions", action);

        return response.guid!;
    }
    public static async Task<string> PutAction(ApiCaller caller, string guid, string name, string actionType, string missionId, int priority, string scopeReference, List<MissionApiSchma.GetActionByGuid.Parameter> parameterList)
    {
        List<dynamic> parameters = [];
        foreach (var parameter in parameterList)
        {
            dynamic actionParameter = new
            {
                guid = parameter.Guid,
                value = parameter.Value,
                id = parameter.Id,
                input_name = parameter.InputName
            };
            parameters.Add(actionParameter);
        }
        dynamic action = new
        {
            name,
            action_type = actionType,
            priority,
            scope_reference = scopeReference,
            parameters
        };
        dynamic response = await caller.PutApi($"missions/{missionId}/actions/{guid}", action);

        return response.guid!;
    }
    public static void DeleteAction(ApiCaller caller, string missionGuid, string actionGuid)
    {
        caller.DeleteApi($"missions/{missionGuid}/actions/{actionGuid}");
    }
    
}