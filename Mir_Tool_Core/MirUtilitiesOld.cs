
/*
using Newtonsoft.Json;

namespace Mir_Utilities;

public class MirUtilitiesOld
{
    private MirRobotApi _robotApi;
    public Boolean ApiStatus = false;

    public MirUtilitiesOld(String robotIp, String authId)
    {
        
        MirRobotApi.MiRRobot robot  = new MirRobotApi.MiRRobot(robotIp, authId);
        _robotApi = new MirRobotApi(robot);
        ApiStatus = _robotApi.VerifyConnection();
    }


    //Will be Deprecated

    public String Export(String name)
    {
        List<GetMapSnapshot>? mapList = _robotApi.GetMap();
        if (mapList == null)
        {
            throw new NullReferenceException("Unable to get Maps from the robot");
        }

        GetMapSnapshot mapSnapshot = mapList.Find(map => map.Name == name);
        if (mapSnapshot.Equals(new GetMapSnapshot()))
        {
            throw new ArgumentException($"Unable to find a map with the name [{name}]");
        }

        Map map = CreateMap(mapSnapshot.Guid).Result;
        return JsonConvert.SerializeObject(map);
    }

    //secondary helper functions


    public async Task<Map> CreateMap(String guid)
    {
        Map map = new Map();
        GetMapByGuidSnapshot mapByGuidSnapshot = await _robotApi.GetMapByGuid(guid);
        List<GetPositionsByMapSnapshot>? positionsByMapSnapshots = await _robotApi.GetPositionsByMap(guid);
        List<GetZonesByMapSnapshot>? zonesByMapSnapshots = await _robotApi.GetZonesByMap(guid);
        List<GetPathGuidesByMapSnapshot>? pathGuidesByMapSnapshots = await _robotApi.GetPathGuidesByMap(guid);
        map.Name = mapByGuidSnapshot.Name;
        map.BaseMap = mapByGuidSnapshot.BaseMap;
        map.SiteId = mapByGuidSnapshot.SiteId;
        map.Guid = mapByGuidSnapshot.Guid;
        map.OriginX = mapByGuidSnapshot.OriginX;
        map.OriginY = mapByGuidSnapshot.OriginY;
        map.Resolution = mapByGuidSnapshot.Resolution;

        if (positionsByMapSnapshots != null)
        {
            List<Map.Position> positionList = new List<Map.Position>();
            foreach (var position in positionsByMapSnapshots)
            {
                Map.Position mapPosition = new Map.Position();
                GetPositionByGuidSnapshot positionSnapshot = await _robotApi.GetPositionByGuid(position.Guid);
                mapPosition.Guid = positionSnapshot.Guid;
                mapPosition.Name = positionSnapshot.Name;
                mapPosition.TypeId = positionSnapshot.TypeId;
                mapPosition.PosX = positionSnapshot.PosX;
                mapPosition.PosY = positionSnapshot.PosY;
                mapPosition.Orientation = positionSnapshot.Orientation;
                List<String>? guidList = await _robotApi.GetHelperPositionsByGuid(positionSnapshot.Guid);
                if (guidList != null)
                {
                    List<Map.Position> helperPositionList = new List<Map.Position>();
                    foreach (var helperGuid in guidList)
                    {
                        Map.Position helperPosition = new Map.Position();
                        GetPositionByGuidSnapshot helperPositionSnapshot = await _robotApi.GetPositionByGuid(helperGuid);

                        helperPosition.Guid = helperPositionSnapshot.Guid;
                        helperPosition.Name = helperPositionSnapshot.Name;
                        helperPosition.TypeId = helperPositionSnapshot.TypeId;
                        helperPosition.PosX = helperPositionSnapshot.PosX;
                        helperPosition.PosY = helperPositionSnapshot.PosY;
                        helperPosition.Orientation = helperPositionSnapshot.Orientation;
                        helperPositionList.Add(helperPosition);
                    }
                    mapPosition.HelperPositions = helperPositionList;
                }
                
                positionList.Add(mapPosition);
            }
            positionList = RemoveDuplicatedHelperPositions(positionList);
            map.Positions = positionList;
        }

        if (zonesByMapSnapshots != null)
        {
            List<Map.Zone> zoneList = new List<Map.Zone>();
            foreach (var zone in zonesByMapSnapshots)
            {
                GetZoneByGuidSnaphot getZoneByGuidSnaphot = await _robotApi.GetZoneByGuid(zone.Guid);
                Map.Zone mapZone = new Map.Zone();
                mapZone.Guid = getZoneByGuidSnaphot.Guid;
                mapZone.Name = getZoneByGuidSnaphot.Name;
                mapZone.ShapeType = getZoneByGuidSnaphot.ShapeType;
                mapZone.TypeId = getZoneByGuidSnaphot.TypeId;
                mapZone.StrokeWidth = getZoneByGuidSnaphot.StrokeWidth;
                mapZone.Direction = getZoneByGuidSnaphot.Direction;
                List<Map.Zone.Coordinates> coordinatesList = new List<Map.Zone.Coordinates>();
                foreach (var coordinate in getZoneByGuidSnaphot.Polygon)
                {
                    Map.Zone.Coordinates mapCoordinate = new Map.Zone.Coordinates();
                    mapCoordinate.X = coordinate.X;
                    mapCoordinate.Y = coordinate.Y;
                    coordinatesList.Add(mapCoordinate);
                }
                mapZone.Polygon = coordinatesList;
                mapZone.Actions = getZoneByGuidSnaphot.Actions;
                zoneList.Add(mapZone);
            }
            map.Zones = zoneList;


        }

        if (pathGuidesByMapSnapshots != null)
        {
            List<Map.PathGuide> pathGuideList = new List<Map.PathGuide>();
            //get pathguide info 
            foreach (var pathGuide in pathGuidesByMapSnapshots)
            {
                GetPathGuideByGuidSnapshot pathGuideSnapshot = await _robotApi.GetPathGuideByGuid(pathGuide.Guid);
                Map.PathGuide mapPathGuide = new Map.PathGuide();
                mapPathGuide.Guid = pathGuideSnapshot.Guid;
                mapPathGuide.Name = pathGuideSnapshot.Name;
                List<Map.PathGuide.PathPosition> pathPositionList = new List<Map.PathGuide.PathPosition>();
                List<String>? guidList = await _robotApi.GetPositionsByPathGuide(pathGuide.Guid);
                if (guidList != null)
                {
                    foreach (var pathPositionGuid in guidList)
                    {
                        GetPositionByPathGuideSnapshot pathPositionSnapshot = await _robotApi.GetPositionByPathGuide(pathGuide.Guid, pathPositionGuid);
                        Map.PathGuide.PathPosition mapPathPosition = new Map.PathGuide.PathPosition();
                        mapPathPosition.Guid = pathPositionSnapshot.Guid;
                        mapPathPosition.PositionGuid = pathPositionSnapshot.PositionsGuid;
                        mapPathPosition.Priority = pathPositionSnapshot.Priority;
                        mapPathPosition.PositionType = pathPositionSnapshot.PositionType;
                        pathPositionList.Add(mapPathPosition);
                    }
                }
                mapPathGuide.PathPositions = pathPositionList;
                pathGuideList.Add(mapPathGuide);
            }
            map.PathGuides = pathGuideList;
        }

        return map;
        
        
        
        /*
        Map map = new Map();
        
        Task getBasicMapInfo = new Task(async () =>
        {
            GetMapByGuidSnapshot mapByGuidSnapshot = await _api.GetMapByGuid(guid);
            map.Name = mapByGuidSnapshot.Name;
            map.BaseMap = mapByGuidSnapshot.BaseMap;
            map.Guid = mapByGuidSnapshot.Guid;
            map.OriginX = mapByGuidSnapshot.OriginX;
            map.OriginY = mapByGuidSnapshot.OriginY;
            map.Resolution = mapByGuidSnapshot.Resolution;
            map.SiteId = mapByGuidSnapshot.SiteId;
        });
        
        Task getMapPositionsInfo = new Task(async () =>
        {
            List<GetPositionsByMapSnapshot>? positionsByMapSnapshots = await _api.GetPositionsByMap(guid);
            if (positionsByMapSnapshots != null)
            {
                List<Map.Position> positionList = new List<Map.Position>();
                List<Task> getPositionInfoTasks = new List<Task>();
                foreach (var position in positionsByMapSnapshots)
                {
                    getPositionInfoTasks.Add(Task.Run(async () =>
                        {
                            Map.Position mapPosition = new Map.Position();
                            
                            Task getBasicPositionInfo = new Task(async () =>
                            {
                                GetPositionByGuidSnapshot
                                    positionSnapshot = await _api.GetPositionByGuid(position.Guid);
                                mapPosition.Guid = positionSnapshot.Guid;
                                mapPosition.Name = positionSnapshot.Name;
                                mapPosition.TypeId = positionSnapshot.TypeId;
                                mapPosition.PosX = positionSnapshot.PosX;
                                mapPosition.PosY = positionSnapshot.PosY;
                                mapPosition.Orientation = positionSnapshot.Orientation;
                            });
                            Task getHelperPositionsInfo = new Task(async () =>
                            {
                                List<String>? guidList = await _api.GetHelperPositionsByGuid(position.Guid);
                                if (guidList != null)
                                {
                                    List<Map.Position> helperPositionList = new List<Map.Position>();
                                    List<Task> getHelperPositionInfoTasks = new List<Task>();
                                    foreach (var helperGuid in guidList)
                                    {
                                        getHelperPositionInfoTasks.Add(Task.Run(async () =>
                                        {
                                            Map.Position helperPosition = new Map.Position();
                                            GetPositionByGuidSnapshot helperPositionSnapshot =
                                                await _api.GetPositionByGuid(helperGuid);

                                            helperPosition.Guid = helperPositionSnapshot.Guid;
                                            helperPosition.Name = helperPositionSnapshot.Name;
                                            helperPosition.TypeId = helperPositionSnapshot.TypeId;
                                            helperPosition.PosX = helperPositionSnapshot.PosX;
                                            helperPosition.PosY = helperPositionSnapshot.PosY;
                                            helperPosition.Orientation = helperPositionSnapshot.Orientation;
                                            helperPositionList.Add(helperPosition);
                                        }));
                                        
                                    }

                                    await Task.WhenAll(getHelperPositionInfoTasks);
                                    mapPosition.HelperPositions = helperPositionList;
                                }
                            });
                            getBasicPositionInfo.Start();
                            getHelperPositionsInfo.Start();
                            await Task.WhenAll(getBasicPositionInfo, getHelperPositionsInfo);
                            positionList.Add(mapPosition);
                        }));

                }

                await Task.WhenAll(getPositionInfoTasks);

                //remove duplicated helper positions
                //positionList = RemoveDuplicatedHelperPositions(positionList);

                map.Positions = positionList;



            }
        });



        Task getMapZonesInfo = new Task(async () =>
        {
            List<GetZonesByMapSnapshot>? zonesByMapSnapshots = await _api.GetZonesByMap(guid);
            if (zonesByMapSnapshots != null)
            {
                List<Map.Zone> zoneList = new List<Map.Zone>();
                List<Task> getZoneInfoTasks = new List<Task>();
                foreach (var zone in zonesByMapSnapshots)
                {
                    getZoneInfoTasks.Add(Task.Run(async () =>
                    {
                        GetZoneByGuidSnaphot getZoneByGuidSnaphot = await _api.GetZoneByGuid(zone.Guid);
                        Map.Zone mapZone = new Map.Zone();
                        mapZone.Guid = getZoneByGuidSnaphot.Guid;
                        mapZone.Name = getZoneByGuidSnaphot.Name;
                        mapZone.ShapeType = getZoneByGuidSnaphot.ShapeType;
                        mapZone.TypeId = getZoneByGuidSnaphot.TypeId;
                        mapZone.StrokeWidth = getZoneByGuidSnaphot.StrokeWidth;
                        mapZone.Direction = getZoneByGuidSnaphot.Direction;
                        List<Map.Zone.Coordinates> coordinatesList = new List<Map.Zone.Coordinates>();
                        foreach (var coordinate in getZoneByGuidSnaphot.Polygon)
                        {
                            Map.Zone.Coordinates mapCoordinate = new Map.Zone.Coordinates();
                            mapCoordinate.X = coordinate.X;
                            mapCoordinate.Y = coordinate.Y;
                            coordinatesList.Add(mapCoordinate);
                        }

                        mapZone.Polygon = coordinatesList;
                        mapZone.Actions = getZoneByGuidSnaphot.Actions;
                        zoneList.Add(mapZone);
                    }));
                }
                await Task.WhenAll(getZoneInfoTasks);
                map.Zones = zoneList;
            }
            
            
        });
        
        Task getMapPathGuidesInfo = new Task(async () =>
        {
            List<GetPathGuidesByMapSnapshot>? pathGuidesByMapSnapshots = await _api.GetPathGuidesByMap(guid);
            if (pathGuidesByMapSnapshots != null)
            {
                List<Map.PathGuide> pathGuideList = new List<Map.PathGuide>();
                List<Task> getPathGuideInfoTasks = new List<Task>();
                foreach (var pathGuide in pathGuidesByMapSnapshots)
                {
                    getPathGuideInfoTasks.Add(Task.Run(async () =>
                    {
                        Map.PathGuide mapPathGuide = new Map.PathGuide();

                        Task getPathGuideBasicInfo = new Task(async () =>
                        {
                            GetPathGuideByGuidSnapshot
                                pathGuideSnapshot = await _api.GetPathGuideByGuid(pathGuide.Guid);
                            mapPathGuide.Guid = pathGuideSnapshot.Guid;
                            mapPathGuide.Name = pathGuideSnapshot.Name;
                        });
                        Task getPathGuidePositionsInfo = new Task(async () =>
                        {
                            List<Map.PathGuide.PathPosition> pathPositionList = new List<Map.PathGuide.PathPosition>();
                            List<String>? guidList = await _api.GetPositionsByPathGuide(pathGuide.Guid);
                            if (guidList != null)
                            {
                                List<Task> getPathGuidePositionsInfoTasks = new List<Task>();
                                foreach (var pathPositionGuid in guidList)
                                {
                                    getPathGuidePositionsInfoTasks.Add(Task.Run(async () =>
                                    {
                                        GetPositionByPathGuideSnapshot pathPositionSnapshot =
                                            await _api.GetPositionByPathGuide(pathGuide.Guid, pathPositionGuid);
                                        Map.PathGuide.PathPosition mapPathPosition = new Map.PathGuide.PathPosition();
                                        mapPathPosition.Guid = pathPositionSnapshot.Guid;
                                        mapPathPosition.PositionGuid = pathPositionSnapshot.PositionsGuid;
                                        mapPathPosition.Priority = pathPositionSnapshot.Priority;
                                        mapPathPosition.PositionType = pathPositionSnapshot.PositionType;
                                        pathPositionList.Add(mapPathPosition);
                                    }));
                                   
                                }
                                await Task.WhenAll(getPathGuidePositionsInfoTasks);
                                mapPathGuide.PathPositions = pathPositionList;
                            }
                            

                        });
                        getPathGuideBasicInfo.Start();
                        getPathGuidePositionsInfo.Start();
                        await Task.WhenAll(getPathGuideBasicInfo, getPathGuidePositionsInfo);

                        pathGuideList.Add(mapPathGuide);
                    }));
                    
                }
                await Task.WhenAll(getPathGuideInfoTasks);
                map.PathGuides = pathGuideList;
            }
        });
        getBasicMapInfo.Start();
        getMapPositionsInfo.Start();
        getMapZonesInfo.Start();
        getMapPathGuidesInfo.Start();

        await Task.WhenAll(getBasicMapInfo, getMapPositionsInfo, getMapZonesInfo, getMapPathGuidesInfo);
        
        return map;
        #1#
    }
    
    private List<Map.Position> RemoveDuplicatedHelperPositions(List<Map.Position> positionList)
    {
        List<Map.Position> helperPositionList = new List<Map.Position>();
        foreach (var position in positionList)
        {
            if (position.HelperPositions != null)
            {
                foreach (var helperPosition in position.HelperPositions)
                {
                    helperPositionList.Add(helperPosition);
                }
            }
        }
        List<Map.Position> uniquePositionList = new List<Map.Position>();
        foreach (var position in positionList)
        {
            if (!helperPositionList.Contains(position))
            {
                uniquePositionList.Add(position);
            }
        }
        return uniquePositionList;
    }
    

    //Will be Deprecated
    public async Task ImportMap(String mapString, Action<String> consoleMessage ,Boolean bypassConfirmation, Func<String,Task<Boolean>> confirmationMessage)
    {
        consoleMessage("starting import process");
        Map map = JsonConvert.DeserializeObject<Map>(mapString);
        try
        {
             GetMapByGuidSnapshot getMapByGuidSnapshot =  await _robotApi.GetMapByGuid(map.Guid);
             if (bypassConfirmation == false)
             {
                 Boolean confirmation =  await confirmationMessage($"Map {map.Name} with the guid {map.Guid} exists on the robot, do you wish to update the map?");
                 if(confirmation == false)
                 {
                     consoleMessage("User has chosen not to update the map");
                     return;
                 }
             }
             consoleMessage("updating map");
             String newGuid = await _robotApi.PutMap(map.Guid, map.Name, map.BaseMap,map.SiteId, map.OriginX, map.OriginY, map.OriginTheta, map.Resolution );
        }catch (HttpRequestException e)
        {
            consoleMessage("Map does not exist on the robot, creating new map");
            String newGuid = await _robotApi.PostMap(map.Guid, map.Name, map.BaseMap,map.SiteId, map.OriginX, map.OriginY, map.OriginTheta, map.Resolution );
        }
        
        if(map.Positions != null)
        {
            foreach (var position in map.Positions)
            {
                try
                {
                    GetPositionByGuidSnapshot getPositionByGuidSnapshot = await _robotApi.GetPositionByGuid(position.Guid);
                    if (bypassConfirmation == false)
                    {
                        Boolean confirmation =  await confirmationMessage($"Position {position.Name} with the guid {position.Guid} exists on the robot, do you wish to update the position?");
                        if(confirmation == false)
                        {
                            consoleMessage("User has chosen not to update the position");
                            continue;
                        }
                    }
                    consoleMessage($"updating position {position.Name}");
                    String newGuid = await _robotApi.PutPosition(position.Guid, position.Name, position.PosX, position.PosY, position.Orientation, position.TypeId, null, map.Guid );
                    if(position.HelperPositions != null)
                    {
                        foreach (var helperPosition in position.HelperPositions)
                        {
                            try
                            {
                                GetPositionByGuidSnapshot getHelperPositionByGuidSnapshot = await _robotApi.GetPositionByGuid(helperPosition.Guid);
                                consoleMessage($"updating helper position {helperPosition.Name}");
                                String newHelperGuid = await _robotApi.PutPosition(helperPosition.Guid, helperPosition.Name, helperPosition.PosX, helperPosition.PosY, helperPosition.Orientation, helperPosition.TypeId, position.Guid, map.Guid );
                            }catch (HttpRequestException e)
                            {
                                consoleMessage($"Helper Position {helperPosition.Name} does not exist on the robot, creating new helper position");
                                String newHelperGuid = await _robotApi.PostPosition(helperPosition.Guid, helperPosition.Name, helperPosition.PosX, helperPosition.PosY, helperPosition.Orientation, helperPosition.TypeId, position.Guid, map.Guid );
                            }
                        }
                    }
                    
                }catch (HttpRequestException e)
                {
                    consoleMessage($"Position {position.Name} does not exist on the robot, creating new position");
                    String newGuid = await _robotApi.PostPosition(position.Guid, position.Name, position.PosX, position.PosY, position.Orientation, position.TypeId, null, map.Guid );
                    
                    
                    //add helper positions
                    if (position.HelperPositions != null)
                    {
                        //Getting default positions generated automatically then deleting them
                        List<String>? oldPositions = await _robotApi.GetHelperPositionsByGuid(newGuid);
                        foreach (var helperPosition in position.HelperPositions )
                        {
                            //create new helper positions
                            String newHelperGuid = await _robotApi.PostPosition(helperPosition.Guid, helperPosition.Name, helperPosition.PosX, helperPosition.PosY, helperPosition.Orientation, helperPosition.TypeId, newGuid, map.Guid );
                        }
                        
                        if (oldPositions != null)
                        {
                            String tempGuid = await _robotApi.PostPosition(null,"temp",0,0,0,0,null,map.Guid);
                            foreach (var oldGuid in oldPositions)
                            {
                                String modPos  = await _robotApi.PutPosition(oldGuid, null, null, null, null, null, tempGuid, null);
                                
                            }
                            _robotApi.DeletePosition(tempGuid);
                        }
                        else
                        {
                            //this should never happen, but just in case
                            Console.WriteLine("Caution! Helper positions not found on the robot, but they are present in the map. This may cause issues.");
                        }
                        
                        
                       
                        
                        
                    }
                    
                    
                    
                    
                }
                
                
               
                
                
            }
        }
        if(map.Zones != null)
        {
            foreach (var zone in map.Zones)
            {
                try
                {
                    GetZoneByGuidSnaphot getZoneByGuidSnaphot = await _robotApi.GetZoneByGuid(zone.Guid);
                    if (bypassConfirmation == false)
                    {
                        Boolean confirmation =  await confirmationMessage($"Zone {zone.Name} with the guid {zone.Guid} exists on the robot, do you wish to update the zone?");
                        if(confirmation == false)
                        {
                            consoleMessage("User has chosen not to update the zone");
                            continue;
                        }
                    }
                    consoleMessage($"updating zone {zone.Name}");
                    String newGuid = await _robotApi.PutZone(zone.Guid, zone.Name, zone.TypeId,  zone.StrokeWidth, zone.Direction, zone.Polygon, zone.Actions, map.Guid);
                }catch (HttpRequestException e)
                {
                    consoleMessage($"Zone {zone.Name} does not exist on the robot, creating new zone");
                    String newGuid = await _robotApi.PostZone(zone.Guid, zone.Name, zone.ShapeType , zone.TypeId,  zone.StrokeWidth, zone.Direction, zone.Polygon, zone.Actions, map.Guid);
                }
            }
        }

        if (map.PathGuides != null)
        {
            foreach (var pathGuide in map.PathGuides)
            {
                try
                {
                    GetPathGuideByGuidSnapshot getPathGuideByGuidSnapshot = await _robotApi.GetPathGuideByGuid(pathGuide.Guid);
                    if (bypassConfirmation == false)
                    {
                        Boolean confirmation =  await confirmationMessage($"Path Guide {pathGuide.Name} with the guid {pathGuide.Guid} exists on the robot, do you wish to update the path guide?");
                        if(confirmation == false)
                        {
                            consoleMessage("User has chosen not to update the path guide");
                            continue;
                        }
                    }
                    consoleMessage($"updating path guide {pathGuide.Name}");
                    String newGuid = await _robotApi.PutPathGuide(pathGuide.Guid, pathGuide.Name, pathGuide.PathPositions, map.Guid );
                }catch (HttpRequestException e)
                {
                    consoleMessage($"Path Guide {pathGuide.Name} does not exist on the robot, creating new path guide");
                    String newGuid = await _robotApi.PostPathGuide(pathGuide.Guid, pathGuide.Name, pathGuide.PathPositions, map.Guid );
                }
            }
        }

        consoleMessage("Completed Import");





    }
}*/