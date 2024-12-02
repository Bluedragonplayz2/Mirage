namespace Mir_Utilities;

public class ZoneApi
{
    public async Task<List<ZoneApiSchema.GetZonesByMapSnapshot>?> GetZonesByMap(ApiCaller caller, String mapId)
    {
        dynamic zoneList = await caller.GetApi($"maps/{mapId}/zones");
        if (zoneList.Count == 0)
        {
            return null;
        }

        List<ZoneApiSchema.GetZonesByMapSnapshot> zoneListSnapshot = new List<ZoneApiSchema.GetZonesByMapSnapshot>();
        foreach (var zone in zoneList)
        {
            ZoneApiSchema.GetZonesByMapSnapshot zoneSnapshot = new ZoneApiSchema.GetZonesByMapSnapshot();
            zoneSnapshot.Name = zone.name!;
            zoneSnapshot.Guid = zone.guid!;
            zoneSnapshot.TypeId = zone.type_id!;
            zoneListSnapshot.Add(zoneSnapshot);
        }

        return zoneListSnapshot;
    }
    
    public async Task<ZoneApiSchema.GetZoneByGuidSnaphot> GetZoneByGuid(ApiCaller caller, String guid)
    {
        dynamic zoneApi = await caller.GetApi($"zones/{guid}");
        ZoneApiSchema.GetZoneByGuidSnaphot zoneSnapshot = new ZoneApiSchema.GetZoneByGuidSnaphot();
        zoneSnapshot.Name = zoneApi.name!;
        zoneSnapshot.Guid = zoneApi.guid!;
        zoneSnapshot.TypeId = zoneApi.type_id!;
        zoneSnapshot.ShapeType = zoneApi.shape_type!;
        zoneSnapshot.StrokeWidth = zoneApi.stroke_width!;
        zoneSnapshot.Direction = zoneApi.direction!;
        zoneSnapshot.Polygon = new List<ZoneApiSchema.GetZoneByGuidSnaphot.Coordinates>();

        foreach (var coord in zoneApi.polygon)
        {
            ZoneApiSchema.GetZoneByGuidSnaphot.Coordinates coordinates = new ZoneApiSchema.GetZoneByGuidSnaphot.Coordinates();
            coordinates.X = coord.x!;
            coordinates.Y = coord.y!;
            zoneSnapshot.Polygon.Add(coordinates);
        }
        
        zoneSnapshot.Actions = zoneApi.actions!;
        return zoneSnapshot;
    }
    public async Task<String> PostZone(ApiCaller caller, String guid, String name, String shapeType, int typeId, float strokeWidth, float direction, List<Map.Zone.Coordinates> polygon, dynamic actions, String mapId)
    {
        dynamic zone = new
        {
            guid,
            name,
            shape_type = shapeType,
            type_id = typeId,
            stroke_width = strokeWidth,
            direction,
            polygon = (polygon.Count > 0)? polygon : null,
            actions,
            map_id = mapId
        };
        dynamic response = await caller.PostApi("zones", zone);
        return response.guid!;
    }
    public async Task<String> PutZone(ApiCaller caller, String guid, String name, int typeId, float strokeWidth, float direction, List<Map.Zone.Coordinates> polygon, dynamic actions, String mapId)
    {
        dynamic zone = new
        {
            name,
            stroke_width = strokeWidth,
            direction,
            polygon = (polygon.Count > 0)? polygon : null,
            actions,
        };
        dynamic response = await caller.PutApi($"zones/{guid}", zone);
        return response.guid!;
    }
}