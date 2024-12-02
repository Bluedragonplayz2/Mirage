
namespace Mir_Utilities;

public class PositionApi
{
    public async Task<List<PositionApiSchema.GetPositionsByMapSnapshot>?> GetPositionsByMap(ApiCaller caller, String mapId)
    {
        dynamic positionList = await caller.GetApi($"maps/{mapId}/positions");
        if (positionList.Count == 0)
        {
            return null;
        }

        List<PositionApiSchema.GetPositionsByMapSnapshot> positionListSnapshot = new List<PositionApiSchema.GetPositionsByMapSnapshot>();

        foreach (var position in positionList)
        {
            PositionApiSchema.GetPositionsByMapSnapshot positionSnapshot = new PositionApiSchema.GetPositionsByMapSnapshot();
            positionSnapshot.Name = position.name!;
            positionSnapshot.Guid = position.guid!;
            positionSnapshot.TypeId = position.type_id!;
            positionListSnapshot.Add(positionSnapshot);
        }

        return positionListSnapshot;
    }
    public async Task<PositionApiSchema.GetPositionByGuidSnapshot> GetPositionByGuid(ApiCaller caller, String guid)
    {
        dynamic positionsApi = await caller.GetApi($"positions/{guid}");
        PositionApiSchema.GetPositionByGuidSnapshot positionSnapshot = new PositionApiSchema.GetPositionByGuidSnapshot();
        positionSnapshot.Name = positionsApi.name!;
        positionSnapshot.Guid = positionsApi.guid!;
        positionSnapshot.PosX = positionsApi.pos_x!;
        positionSnapshot.PosY = positionsApi.pos_y!;
        positionSnapshot.Orientation = positionsApi.orientation!;
        positionSnapshot.TypeId = positionsApi.type_id!;
        positionSnapshot.ParentId = positionsApi.parent_id;
        
        return positionSnapshot;
    }
    public async Task<List<String>?> GetHelperPositionsByGuid(ApiCaller caller, String guid)
    {
        dynamic helperPositionsApi = await caller.GetApi($"positions/{guid}/helper_positions");
        if (helperPositionsApi.Count == 0)
        {
            return null;
        }
        List<String> guidList = new List<string>();
        foreach (var position in helperPositionsApi)
        {
            guidList.Add(position.guid!.ToString());
        }

        return guidList;
    }
    public async Task<List<String>?> GetPositionsByPathGuide(ApiCaller caller, String pathGuideGuid)
    {
        dynamic positionsApi = await caller.GetApi($"path_guides/{pathGuideGuid}/positions");
        if (positionsApi.Count == 0)
        {
            return null;
        }
        List<String> guidList = new List<string>();
        foreach (var position in positionsApi)
        {
            guidList.Add(position.guid!.ToString());
        }

        return guidList;
    }

    public async Task<PositionApiSchema.GetPositionByPathGuideSnapshot> GetPositionByPathGuide(ApiCaller caller, String pathGuideGuid, String guid)
    {
        dynamic positionsApi = await caller.GetApi($"path_guides/{pathGuideGuid}/positions/{guid}");
        PositionApiSchema.GetPositionByPathGuideSnapshot positionSnapshot = new PositionApiSchema.GetPositionByPathGuideSnapshot();
        positionSnapshot.Guid = positionsApi.guid!;
        positionSnapshot.PositionsGuid = positionsApi.pos_guid!;
        positionSnapshot.PositionType = positionsApi.pos_type!;
        positionSnapshot.Priority = positionsApi.priority!;
        return positionSnapshot;
    }
    
    public async Task<String> PostPosition(ApiCaller caller, String guid, String name, float posX, float posY, float orientation, int typeId, String? parentId, String mapId)
    {
        dynamic position = new
        {
            guid,
            name,
            pos_x = posX,
            pos_y = posY,
            orientation,
            offset_x = posX,
            offset_y = posY,
            offset_orientation = orientation,
            type_id = typeId,
            map_id = mapId,
            parent_id = parentId
        };
        dynamic response = await caller.PostApi("positions", position);
        return response.guid!;
    }
    
    public async Task<String> PutPosition(ApiCaller caller, String guid, String? name, float? posX, float? posY, float? orientation, int? typeId, String? parentId, String? mapId)
    {
        dynamic position = new
        {
            name,
            pos_x = posX,
            pos_y = posY,
            orientation,
            type_id = typeId,
            map_id = mapId,
            parent_id = parentId
        };
        dynamic response = await caller.PutApi($"positions/{guid}", position);
        return response.guid!;
    }
    public void DeletePosition(ApiCaller caller, String guid)
    {
        caller.DeleteApi($"positions/{guid}");
    }
    

}