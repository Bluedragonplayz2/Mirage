namespace Mir_Utilities;

public class MapApi
{
    public static async Task<List<MapApiSchema.GetMapSnapshot>?> GetMap(ApiCaller caller)
    {
        dynamic mapList = await caller.GetApi("maps");
        if (mapList.Count == 0)
        {
            return null;
        }

        List<MapApiSchema.GetMapSnapshot> mapListSnapshot = new List<MapApiSchema.GetMapSnapshot>();
        foreach (var map in mapList)
        {
            MapApiSchema.GetMapSnapshot mapSnapshot = new MapApiSchema.GetMapSnapshot();
            mapSnapshot.Name = (map.name != null) ? map.name : "Unnamed Map";
            mapSnapshot.Guid = (map.guid != null) ? map.guid : "Unknown Guid";
            mapListSnapshot.Add(mapSnapshot);
        }

        return mapListSnapshot;
    }

    public static async Task<MapApiSchema.GetMapByGuidSnapshot> GetMapByGuid(ApiCaller caller, String guid)
    {
        dynamic map = await caller.GetApi($"maps/{guid}");
        MapApiSchema.GetMapByGuidSnapshot mapSnapshot = new MapApiSchema.GetMapByGuidSnapshot();
        mapSnapshot.Guid = map.guid!;
        mapSnapshot.BaseMap = map.base_map!;
        mapSnapshot.SiteId = map.session_id!;
        mapSnapshot.Name = map.name!;
        mapSnapshot.OriginX = map.origin_x!;
        mapSnapshot.OriginY = map.origin_y!;
        mapSnapshot.OriginTheta = map.origin_theta!;
        mapSnapshot.Resolution = map.resolution!;
        return mapSnapshot;
    }
    
    public static  async Task<String> PostMap(ApiCaller caller, String guid, String mapName, String baseMap, String siteId, float originX, float originY, float originTheta, float resolution)
    {
        dynamic map = new
        {
            guid,
            name = mapName,
            base_map = baseMap,
            session_id = siteId,
            origin_x = originX,
            origin_y = originY,
            origin_theta = originTheta,
            resolution
        };
        dynamic response = await caller.PostApi("maps", map);
        return response.guid!;
    }
    public static async Task<String> PutMap(ApiCaller caller,  String guid, String mapName, String baseMap, String siteId, float originX, float originY, float originTheta, float resolution)
    {
        dynamic map = new
        {
            name = mapName,
            base_map = baseMap,
            origin_x = originX,
            origin_y = originY,
            origin_theta = originTheta,
            resolution
        };
        dynamic response = await caller.PutApi($"maps/{guid}", map);
        return response.guid!;
    }
}