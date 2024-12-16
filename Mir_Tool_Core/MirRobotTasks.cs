namespace Mir_Utilities;

public class MiRRobotApi
{
    private MirRobotApi _robotApi;
    
    public MiRRobotApi( MirRobotApi robotApi)
    {
        _robotApi = robotApi;
    }
    
    //helper function to handle creation default success message;
    public UpdateEventArgs CreateUpdateMessage(String statusMsg)
    {
        UpdateEventArgs args = new UpdateEventArgs();
        args.Status = UpdateEventArgs.TaskStatus.RUNNING;
        args.StatusMsg = statusMsg;
        return args;
    }
    
    
    public async Task<Map> CreateMap(String guid, Action<UpdateEventArgs> updateCallback)
    {
        //Todo: add UpdateCallback to update the user on the progress of the operation
        //gets basic map information
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
            
            //Checks for duplicated positions due to helper positions
            List<Map.Position> tempPositionList = positionList;
            foreach (var position in positionList)
            {
                if (position.HelperPositions != null)
                {
                    
                    foreach (var helperPosition in position.HelperPositions)
                    {
                        tempPositionList.Remove(helperPosition);
                    }
                }
            }
            positionList = tempPositionList;
            
            
            //Note: explore if removing position by parent guid is possible
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
            //get path guide info 
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
                        GetPositionByPathGuideSnapshot pathPositionSnapshot =
                            await _robotApi.GetPositionByPathGuide(pathGuide.Guid, pathPositionGuid);
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

    }

    public async void ImportMap(Map map, Boolean  bypassConfirmation, Func<String, Task<Boolean>>? confirmationMessage, Action<UpdateEventArgs> consoleMessage)
    {
        
        try
        {
             await _robotApi.GetMapByGuid(map.Guid);
             if (bypassConfirmation == false)
             {
                 Boolean confirmation =  await confirmationMessage($"Map {map.Name} with the guid {map.Guid} exists on the robot, do you wish to update the map?");
                 if(confirmation == false)
                 {
                     consoleMessage(CreateUpdateMessage("User has chosen not to update the map"));
                     return;
                 }
             }
             consoleMessage(CreateUpdateMessage("updating map"));
             await _robotApi.PutMap(map.Guid, map.Name, map.BaseMap,map.SiteId, map.OriginX, map.OriginY, map.OriginTheta, map.Resolution );
        }catch (HttpRequestException)
        {
            consoleMessage(CreateUpdateMessage("Map does not exist on the robot, creating new map"));
            await _robotApi.PostMap(map.Guid, map.Name, map.BaseMap,map.SiteId, map.OriginX, map.OriginY, map.OriginTheta, map.Resolution );
        }
        
        if(map.Positions != null)
        {
            foreach (var position in map.Positions)
            {
                try
                {
                    await _robotApi.GetPositionByGuid(position.Guid);
                    if (bypassConfirmation == false)
                    {
                        Boolean confirmation =  await confirmationMessage($"Position {position.Name} with the guid {position.Guid} exists on the robot, do you wish to update the position?");
                        if(confirmation == false)
                        {
                            consoleMessage(CreateUpdateMessage("User has chosen not to update the position"));
                            continue;
                        }
                    }
                    consoleMessage(CreateUpdateMessage($"updating position {position.Name}"));
                    await _robotApi.PutPosition(position.Guid, position.Name, position.PosX, position.PosY, position.Orientation, position.TypeId, null, map.Guid );
                    if(position.HelperPositions != null)
                    {
                        foreach (var helperPosition in position.HelperPositions)
                        {
                            try
                            {
                                await _robotApi.GetPositionByGuid(helperPosition.Guid);
                                consoleMessage(CreateUpdateMessage($"updating helper position {helperPosition.Name}"));
                                await _robotApi.PutPosition(helperPosition.Guid, helperPosition.Name, helperPosition.PosX, helperPosition.PosY, helperPosition.Orientation, helperPosition.TypeId, position.Guid, map.Guid );
                            }catch (HttpRequestException)
                            {
                                consoleMessage(CreateUpdateMessage($"Helper Position {helperPosition.Name} does not exist on the robot, creating new helper position"));
                                await _robotApi.PostPosition(helperPosition.Guid, helperPosition.Name, helperPosition.PosX, helperPosition.PosY, helperPosition.Orientation, helperPosition.TypeId, position.Guid, map.Guid );
                            }
                        }
                    }
                    
                }catch (HttpRequestException)
                {
                    consoleMessage(CreateUpdateMessage($"Position {position.Name} does not exist on the robot, creating new position"));
                    String newGuid = await _robotApi.PostPosition(position.Guid, position.Name, position.PosX, position.PosY, position.Orientation, position.TypeId, null, map.Guid );
                    
                    
                    //add helper positions
                    if (position.HelperPositions != null)
                    {
                        //Getting default positions generated automatically then deleting them
                        List<String>? oldPositions = await _robotApi.GetHelperPositionsByGuid(newGuid);
                        foreach (var helperPosition in position.HelperPositions )
                        {
                            //create new helper positions
                            await _robotApi.PostPosition(helperPosition.Guid, helperPosition.Name, helperPosition.PosX, helperPosition.PosY, helperPosition.Orientation, helperPosition.TypeId, newGuid, map.Guid );
                        }
                        
                        if (oldPositions != null)
                        {
                            String tempGuid = await _robotApi.PostPosition(null,"temp",0,0,0,0,null,map.Guid);
                            foreach (var oldGuid in oldPositions)
                            {
                                await _robotApi.PutPosition(oldGuid, null, null, null, null, null, tempGuid, null);
                                
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
                    await _robotApi.GetZoneByGuid(zone.Guid);
                    if (bypassConfirmation == false)
                    {
                        Boolean confirmation =  await confirmationMessage($"Zone {zone.Name} with the guid {zone.Guid} exists on the robot, do you wish to update the zone?");
                        if(confirmation == false)
                        {
                            consoleMessage(CreateUpdateMessage("User has chosen not to update the zone"));
                            continue;
                        }
                    }
                    consoleMessage(CreateUpdateMessage($"updating zone {zone.Name}"));
                    await _robotApi.PutZone(zone.Guid, zone.Name, zone.TypeId,  zone.StrokeWidth, zone.Direction, zone.Polygon, zone.Actions, map.Guid);
                }catch (HttpRequestException)
                {
                    consoleMessage(CreateUpdateMessage($"Zone {zone.Name} does not exist on the robot, creating new zone"));
                    await _robotApi.PostZone(zone.Guid, zone.Name, zone.ShapeType , zone.TypeId,  zone.StrokeWidth, zone.Direction, zone.Polygon, zone.Actions, map.Guid);
                }
            }
        }

        if (map.PathGuides != null)
        {
            foreach (var pathGuide in map.PathGuides)
            {
                try
                {
                    await _robotApi.GetPathGuideByGuid(pathGuide.Guid);
                    if (bypassConfirmation == false)
                    {
                        Boolean confirmation =  await confirmationMessage($"Path Guide {pathGuide.Name} with the guid {pathGuide.Guid} exists on the robot, do you wish to update the path guide?");
                        if(confirmation == false)
                        {
                            consoleMessage(CreateUpdateMessage("User has chosen not to update the path guide"));
                            continue;
                        }
                    }
                    consoleMessage(CreateUpdateMessage($"updating path guide {pathGuide.Name}"));
                    await _robotApi.PutPathGuide(pathGuide.Guid, pathGuide.Name, pathGuide.PathPositions, map.Guid );
                }catch (HttpRequestException)
                {
                    consoleMessage(CreateUpdateMessage($"Path Guide {pathGuide.Name} does not exist on the robot, creating new path guide"));
                   await _robotApi.PostPathGuide(pathGuide.Guid, pathGuide.Name, pathGuide.PathPositions, map.Guid );
                }
            }
        }

        consoleMessage(CreateUpdateMessage("Completed Import"));





    
        
        
    }
}