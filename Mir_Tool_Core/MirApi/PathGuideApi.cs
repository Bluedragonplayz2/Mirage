using Mir_Utilities.Common;

namespace Mir_Utilities.MirApi;

public class PathGuideApi
{
    public static async Task<List<PathGuideApiSchema.GetPathGuidesByMapSnapshot>?> GetPathGuidesByMap(ApiCaller caller, String mapId)
    {
        dynamic pathGuideList = await caller.GetApi($"maps/{mapId}/path_guides");
        if (pathGuideList.Count == 0)
        {
            return null;
        }

        List<PathGuideApiSchema.GetPathGuidesByMapSnapshot> pathGuideListSnapshot = new List<PathGuideApiSchema.GetPathGuidesByMapSnapshot>();
        foreach (var pathGuide in pathGuideList)
        {
            PathGuideApiSchema.GetPathGuidesByMapSnapshot pathGuideSnapshot = new PathGuideApiSchema.GetPathGuidesByMapSnapshot();
            pathGuideSnapshot.Name = pathGuide.name!;
            pathGuideSnapshot.Guid = pathGuide.guid!;
            pathGuideListSnapshot.Add(pathGuideSnapshot);
        }

        return pathGuideListSnapshot;
    }

    public static async Task<PathGuideApiSchema.GetPathGuideByGuidSnapshot> GetPathGuideByGuid(ApiCaller caller, String guid)
    {
        dynamic pathGuideApi = await caller.GetApi($"path_guides/{guid}");
        PathGuideApiSchema.GetPathGuideByGuidSnapshot pathGuideSnapshot = new PathGuideApiSchema.GetPathGuideByGuidSnapshot();
        pathGuideSnapshot.Guid = pathGuideApi.guid!;
        pathGuideSnapshot.Name = pathGuideApi.name!;
        return pathGuideSnapshot;
    }
     public static async Task<String> PostPathGuide(ApiCaller caller, String guid, String name, List<Map.PathGuide.PathPosition> pathPositions, String mapId)
    {
        dynamic pathGuide = new
        {
            guid,
            name,
            map_id = mapId
        };
        dynamic response = await caller.PostApi("path_guides", pathGuide);
        foreach (var position in pathPositions)
        {
            dynamic pathPosition = new
            {
                guid = position.Guid,
                path_guide_guid = guid,
                pos_guid = position.PositionGuid,
                priority = position.Priority,
                pos_type = position.PositionType
            };
            await caller.PostApi($"path_guides/{guid}/positions", pathPosition);
        }
        return response.guid!;
    }
     
    public static async Task<String> PutPathGuide(ApiCaller caller, String guid, String name, List<Map.PathGuide.PathPosition> pathPositions, String mapId)
    {
        dynamic pathGuide = new
        {
            name,
            map_id = mapId
        };
        dynamic response = await caller.PutApi($"path_guides/{guid}", pathGuide);
        //check if positions already exists
        List<String>? positions = await PositionApi.GetPositionsByPathGuide(caller, guid);
        if (positions!.Any())
        {
            foreach (var pathPosition in pathPositions)
            {
                if (positions!.Contains(pathPosition.Guid))
                {
                    await Task.Delay(100);
                    DeletePathGuidePosition(caller, guid, pathPosition.Guid);
                }
            
            }
        }
        foreach (var position in pathPositions)
        {
            dynamic pathPosition = new
            {
                guid = position.Guid,
                path_guide_guid = guid,
                pos_guid = position.PositionGuid,
                priority = position.Priority,
                pos_type = position.PositionType
            };
            await caller.PostApi($"path_guides/{guid}/positions", pathPosition);
        }
        
        return response.guid!;
    }
    public static void DeletePathGuidePosition(ApiCaller caller, String pathGuideGuid, String positionGuid)
    {
        caller.DeleteApi($"path_guides/{pathGuideGuid}/positions/{positionGuid}");
    }
}