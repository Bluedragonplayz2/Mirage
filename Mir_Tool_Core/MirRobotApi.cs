namespace Mir_Utilities;

public class MirRobotApi
{
    public struct MiRRobot(String ip, String authId) //Includes the IP and Generated authId for the robot
    {
        public readonly String Ip { get; } = ip;
        public readonly String AuthId { get; } = authId;

    }
    
    private static ApiCaller _api;
    
    //initialize the API caller with the robot's IP and authId 
    public MirRobotApi(MiRRobot robot)
    {
         _api = new ApiCaller(robot.Ip, robot.AuthId);
        if(VerifyConnection())
        {
            throw new Exception("Failed to create API caller");
        }
        
    }
    
    //future addition to use tokens to avoid tossing around authIds

    
    public Boolean VerifyConnection()
    {
        try
        {
            _api.GetApi("status").Wait();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    public async Task<Boolean> VerifyConnectionAsync()
    {
        try
        {
            await _api.GetApi("status");
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    public List<GetMapSnapshot>? GetMap()
    {
        dynamic mapList = _api.GetApi("maps").Result;
        if (mapList.Count == 0)
        {
            return null;
        }

        List<GetMapSnapshot> mapListSnapshot = new List<GetMapSnapshot>();
        foreach (var map in mapList)
        {
            GetMapSnapshot mapSnapshot = new GetMapSnapshot();
            mapSnapshot.Name = (map.name != null) ? map.name : "Unnamed Map";
            mapSnapshot.Guid = (map.guid != null) ? map.guid : "Unknown Guid";
            mapListSnapshot.Add(mapSnapshot);
        }

        return mapListSnapshot;
    }

    public async Task<GetMapByGuidSnapshot> GetMapByGuid(String guid)
    {
        dynamic map = await _api.GetApi($"maps/{guid}");
        GetMapByGuidSnapshot mapSnapshot = new GetMapByGuidSnapshot();
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

    public async Task<List<GetZonesByMapSnapshot>?> GetZonesByMap(String mapId)
    {
        dynamic zoneList = await _api.GetApi($"maps/{mapId}/zones");
        if (zoneList.Count == 0)
        {
            return null;
        }

        List<GetZonesByMapSnapshot> zoneListSnapshot = new List<GetZonesByMapSnapshot>();
        foreach (var zone in zoneList)
        {
            GetZonesByMapSnapshot zoneSnapshot = new GetZonesByMapSnapshot();
            zoneSnapshot.Name = zone.name!;
            zoneSnapshot.Guid = zone.guid!;
            zoneSnapshot.TypeId = zone.type_id!;
            zoneListSnapshot.Add(zoneSnapshot);
        }

        return zoneListSnapshot;
    }

    public async Task<List<GetPositionsByMapSnapshot>?> GetPositionsByMap(String mapId)
    {
        dynamic positionList = await _api.GetApi($"maps/{mapId}/positions");
        if (positionList.Count == 0)
        {
            return null;
        }

        List<GetPositionsByMapSnapshot> positionListSnapshot = new List<GetPositionsByMapSnapshot>();

        foreach (var position in positionList)
        {
            GetPositionsByMapSnapshot positionSnapshot = new GetPositionsByMapSnapshot();
            positionSnapshot.Name = position.name!;
            positionSnapshot.Guid = position.guid!;
            positionSnapshot.TypeId = position.type_id!;
            positionListSnapshot.Add(positionSnapshot);
        }

        return positionListSnapshot;
    }

    public async Task<List<GetPathGuidesByMapSnapshot>?> GetPathGuidesByMap(String mapId)
    {
        dynamic pathGuideList = await _api.GetApi($"maps/{mapId}/path_guides");
        if (pathGuideList.Count == 0)
        {
            return null;
        }

        List<GetPathGuidesByMapSnapshot> pathGuideListSnapshot = new List<GetPathGuidesByMapSnapshot>();
        foreach (var pathGuide in pathGuideList)
        {
            GetPathGuidesByMapSnapshot pathGuideSnapshot = new GetPathGuidesByMapSnapshot();
            pathGuideSnapshot.Name = pathGuide.name!;
            pathGuideSnapshot.Guid = pathGuide.guid!;
            pathGuideListSnapshot.Add(pathGuideSnapshot);
        }

        return pathGuideListSnapshot;
    }

    public async Task<GetPositionByGuidSnapshot> GetPositionByGuid(String guid)
    {
        dynamic positionsApi = await _api.GetApi($"positions/{guid}");
        GetPositionByGuidSnapshot positionSnapshot = new GetPositionByGuidSnapshot();
        positionSnapshot.Name = positionsApi.name!;
        positionSnapshot.Guid = positionsApi.guid!;
        positionSnapshot.PosX = positionsApi.pos_x!;
        positionSnapshot.PosY = positionsApi.pos_y!;
        positionSnapshot.Orientation = positionsApi.orientation!;
        positionSnapshot.TypeId = positionsApi.type_id!;
        positionSnapshot.ParentId = positionsApi.parent_id;
        
        return positionSnapshot;
    }

    public async Task<List<String>?> GetHelperPositionsByGuid(String guid)
    {
        dynamic helperPositionsApi = await _api.GetApi($"positions/{guid}/helper_positions");
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

    public async Task<GetZoneByGuidSnaphot> GetZoneByGuid(String guid)
    {
        dynamic zoneApi = await _api.GetApi($"zones/{guid}");
        GetZoneByGuidSnaphot zoneSnapshot = new GetZoneByGuidSnaphot();
        zoneSnapshot.Name = zoneApi.name!;
        zoneSnapshot.Guid = zoneApi.guid!;
        zoneSnapshot.TypeId = zoneApi.type_id!;
        zoneSnapshot.ShapeType = zoneApi.shape_type!;
        zoneSnapshot.StrokeWidth = zoneApi.stroke_width!;
        zoneSnapshot.Direction = zoneApi.direction!;
        zoneSnapshot.Polygon = new List<GetZoneByGuidSnaphot.Coordinates>();

            foreach (var coord in zoneApi.polygon)
            {
                GetZoneByGuidSnaphot.Coordinates coordinates = new GetZoneByGuidSnaphot.Coordinates();
                coordinates.X = coord.x!;
                coordinates.Y = coord.y!;
                zoneSnapshot.Polygon.Add(coordinates);
            }
        
        zoneSnapshot.Actions = zoneApi.actions!;
        return zoneSnapshot;
    }
    
    public async Task<GetPathGuideByGuidSnapshot> GetPathGuideByGuid(String guid)
    {
        dynamic pathGuideApi = await _api.GetApi($"path_guides/{guid}");
        GetPathGuideByGuidSnapshot pathGuideSnapshot = new GetPathGuideByGuidSnapshot();
        pathGuideSnapshot.Guid = pathGuideApi.guid!;
        pathGuideSnapshot.Name = pathGuideApi.name!;
        return pathGuideSnapshot;
    }
    
    public async Task<List<String>?> GetPositionsByPathGuide(String pathGuideGuid)
    {
        dynamic positionsApi = await _api.GetApi($"path_guides/{pathGuideGuid}/positions");
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

    public async Task<GetPositionByPathGuideSnapshot> GetPositionByPathGuide(String pathGuideGuid, String guid)
    {
        dynamic positionsApi = await _api.GetApi($"path_guides/{pathGuideGuid}/positions/{guid}");
        GetPositionByPathGuideSnapshot positionSnapshot = new GetPositionByPathGuideSnapshot();
        positionSnapshot.Guid = positionsApi.guid!;
        positionSnapshot.PositionsGuid = positionsApi.pos_guid!;
        positionSnapshot.PositionType = positionsApi.pos_type!;
        positionSnapshot.Priority = positionsApi.priority!;
        return positionSnapshot;
    }
    



        
        
    public async Task<String> PostMap(String guid, String mapName, String baseMap, String siteId, float originX, float originY, float originTheta, float resolution)
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
        dynamic response = await _api.PostApi("maps", map);
        return response.guid!;
    }
    public async Task<String> PutMap(String guid, String mapName, String baseMap, String siteId, float originX, float originY, float originTheta, float resolution)
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
        dynamic response = await _api.PutApi($"maps/{guid}", map);
        return response.guid!;
    }
    public async Task<String> PostPosition(String guid, String name, float posX, float posY, float orientation, int typeId, String? parentId, String mapId)
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
        dynamic response = await _api.PostApi("positions", position);
        return response.guid!;
    }
    public async Task<String> PutPosition(String guid, String? name, float? posX, float? posY, float? orientation, int? typeId, String? parentId, String? mapId)
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
        dynamic response = await _api.PutApi($"positions/{guid}", position);
        return response.guid!;
    }
    public async Task<String> PostZone(String guid, String name, String shapeType, int typeId, float strokeWidth, float direction, List<Map.Zone.Coordinates> polygon, dynamic actions, String mapId)
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
        dynamic response = await _api.PostApi("zones", zone);
        return response.guid!;
    }
        
    public async Task<String> PutZone(String guid, String name, int typeId, float strokeWidth, float direction, List<Map.Zone.Coordinates> polygon, dynamic actions, String mapId)
    {
        dynamic zone = new
        {
            name,
            stroke_width = strokeWidth,
            direction,
            polygon = (polygon.Count > 0)? polygon : null,
            actions,
        };
        dynamic response = await _api.PutApi($"zones/{guid}", zone);
        return response.guid!;
    }
    public async Task<String> PostPathGuide(String guid, String name, List<Map.PathGuide.PathPosition> pathPositions, String mapId)
    {
        dynamic pathGuide = new
        {
            guid,
            name,
            map_id = mapId
        };
        dynamic response = await _api.PostApi("path_guides", pathGuide);
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
            await _api.PostApi($"path_guides/{guid}/positions", pathPosition);
        }
        return response.guid!;
    }
    public async Task<String> PutPathGuide(String guid, String name, List<Map.PathGuide.PathPosition> pathPositions, String mapId)
    {
        dynamic pathGuide = new
        {
            name,
            map_id = mapId
        };
        dynamic response = await _api.PutApi($"path_guides/{guid}", pathGuide);
        //check if positions already exists
        List<String>? positions = await GetPositionsByPathGuide(guid);
        if (positions!.Any())
        {
            foreach (var pathPosition in pathPositions)
            {
                if (positions!.Contains(pathPosition.Guid))
                {
                    await Task.Delay(100);
                    DeletePathGuidePosition(guid, pathPosition.Guid);
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
            await _api.PostApi($"path_guides/{guid}/positions", pathPosition);
        }
        
        return response.guid!;
    }
    public void DeletePathGuidePosition(String pathGuideGuid, String positionGuid)
    {
        _api.DeleteApi($"path_guides/{pathGuideGuid}/positions/{positionGuid}");
    }
    public void DeletePosition(String guid)
    {
        _api.DeleteApi($"positions/{guid}");
    }
}