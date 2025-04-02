using System.Collections.Concurrent;
using System.Security.Authentication;
using Mir_Utilities.Common;
using Mir_Utilities.MirApi;
using RosSharp.RosBridgeClient.MessageTypes.BuiltinInterfaces;

namespace Mir_Utilities.ToolFunction;

public class HandleMap
{
    private static readonly log4net.ILog logger =
        log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public static async Task<Map> ExportMap(RobotSchema.Robot robot, string name, Action<string>? updateStatus = null)
    {
        //Export a single map from the robot
        try
        {
            logger.Info("Setting up API Caller");
            updateStatus?.Invoke("Setting up API Caller");
            ApiCaller caller = new ApiCaller(robot.Ip, robot.AuthId);
            logger.Info("starting export");
            List<MapApiSchema.GetMapSnapshot>? mapList = await MapApi.GetMap(caller);
            logger.Info("gotten map");
            logger.Info(mapList.Count);
            if (mapList == null)
            {
                logger.Info("Unable to get Maps from the robot");
                throw new NullReferenceException("Unable to get Maps from the robot");
            }

            MapApiSchema.GetMapSnapshot mapSnapshot = mapList.Find(map => map.Name == name);
            if (mapSnapshot.Equals(new MapApiSchema.GetMapSnapshot()))
            {
                logger.Info("no map found");
                throw new ArgumentException($"Unable to find a map with the name [{name}]");
            }

            string guid = mapSnapshot.Guid;
            
            logger.Info("Getting Map Info");
            updateStatus?.Invoke("Getting Map Info");
            Map map = new Map();
            List<Task> populateMapTasks =
            [
                PopulateMapDetail(),
                PopulatePositions(),
                PopulateZones(),
                PopulatePathGuides()
            ];
            await Task.WhenAll(populateMapTasks);
            logger.Info("Completed Exporting Map");
            updateStatus?.Invoke("Completed Exporting Map");
            return map;


            async Task PopulateMapDetail()
            {
                logger.Info("Populating Map Detail");
                updateStatus?.Invoke("Populating Map Detail");
                MapApiSchema.GetMapByGuidSnapshot mapByGuidSnapshot =
                    await MapApi.GetMapByGuid(caller, guid) ??
                    throw new InvalidOperationException("Map not found in robot");
                map.SetBasicMapData(
                    mapByGuidSnapshot.Guid,
                    mapByGuidSnapshot.Name,
                    mapByGuidSnapshot.SiteId,
                    mapByGuidSnapshot.OriginX,
                    mapByGuidSnapshot.OriginY,
                    mapByGuidSnapshot.Resolution,
                    mapByGuidSnapshot.OriginTheta,
                    mapByGuidSnapshot.BaseMap
                );
                logger.Info("Finished Populating Map Detail");
                updateStatus?.Invoke("Finished Populating Map Detail");
            }

            async Task PopulatePositions()
            {
                logger.Info("Populating Positions Detail");
                updateStatus?.Invoke("Populating Positions Detail");
                List<PositionApiSchema.GetPositionsByMapSnapshot>? positionsByMapSnapshots =
                    await PositionApi.GetPositionsByMap(caller, guid);
                if (positionsByMapSnapshots == null)
                {
                    logger.Info("No positions found in map");
                    updateStatus?.Invoke("No positions found in map");
                    return;
                }

                ConcurrentBag<Map.Position> positionList = new ConcurrentBag<Map.Position>();
                List<Task> getPositionInfoTasks = new List<Task>();
                foreach (var position in positionsByMapSnapshots)
                {
                    Thread.Sleep(100);
                    getPositionInfoTasks.Add(PopulatePositionDetail(position));
                }

                await Task.WhenAll(getPositionInfoTasks);

                //remove duplicated helper positions
                //positionList = RemoveDuplicatedHelperPositions(positionList);

                map.Positions = positionList.ToArray();
                logger.Info("Finished Populating Positions Detail");
                updateStatus?.Invoke("Finished Populating Positions Detail");
                return;

                async Task PopulatePositionDetail(PositionApiSchema.GetPositionsByMapSnapshot position)
                {
                    Map.Position mapPosition = new Map.Position();
                    PositionApiSchema.GetPositionByGuidSnapshot
                        positionSnapshot = await PositionApi.GetPositionByGuid(caller, position.Guid)?? throw new InvalidOperationException("position not found in robot") ;
                    mapPosition.SetPositionData(
                        positionSnapshot.Guid,
                        positionSnapshot.Name,
                        positionSnapshot.PosX,
                        positionSnapshot.PosY,
                        positionSnapshot.Orientation,
                        positionSnapshot.TypeId
                    );
                    List<String>? guidList = await PositionApi.GetHelperPositionsByGuid(caller, position.Guid);
                    mapPosition.HelperPositionsGuid = guidList?.ToArray() ?? [];
                    positionList.Add(mapPosition);
                    logger.Info($"Added Position: {mapPosition.Name}");
                    updateStatus?.Invoke($"Added Position: {mapPosition.Name}");
                }
            }

            async Task PopulateZones()
            {
                logger.Info("Populating Zones Detail");
                updateStatus?.Invoke("Populating Zones Detail");
                List<ZoneApiSchema.GetZonesByMapSnapshot>? zonesByMapSnapshots =
                    await ZoneApi.GetZonesByMap(caller, guid);
                if (zonesByMapSnapshots == null)
                {
                    logger.Info("No zones found in map");
                    updateStatus?.Invoke("No zones found in map");
                    return;
                }

                ConcurrentBag<Map.Zone> zoneList = new ConcurrentBag<Map.Zone>();
                List<Task> getZoneInfoTasks = new List<Task>();
                foreach (var zone in zonesByMapSnapshots)
                {
                    Thread.Sleep(100);
                    getZoneInfoTasks.Add(PopulateZoneDetail(zone));
                }

                await Task.WhenAll(getZoneInfoTasks);
                map.Zones = zoneList.ToArray();
                logger.Info("Finished Populating Zones Detail");
                updateStatus?.Invoke("Finished Populating Zones Detail");
                return;

                async Task PopulateZoneDetail(ZoneApiSchema.GetZonesByMapSnapshot zone)
                {
                    Map.Zone mapZone = new Map.Zone();
                    ZoneApiSchema.GetZoneByGuidSnaphot zoneSnapshot =
                        await ZoneApi.GetZoneByGuid(caller, zone.Guid) ??
                        throw new InvalidOperationException("Zone not found in robot");
                    List<Map.Zone.Coordinates> coordinates = new List<Map.Zone.Coordinates>();
                    foreach (ZoneApiSchema.GetZoneByGuidSnaphot.Coordinates cord in zoneSnapshot.Polygon)
                    {
                        Thread.Sleep(100);
                        Map.Zone.Coordinates coordinate = new Map.Zone.Coordinates(cord.X, cord.Y);
                        coordinates.Add(coordinate);
                    }

                    mapZone.SetZoneData(
                        zoneSnapshot.Guid,
                        zoneSnapshot.Name,
                        zoneSnapshot.ShapeType,
                        zoneSnapshot.TypeId,
                        zoneSnapshot.StrokeWidth,
                        zoneSnapshot.Direction,
                        coordinates.ToArray(),
                        zoneSnapshot.Actions
                    );
                    zoneList.Add(mapZone);
                    logger.Info($"Added Zone: {mapZone.Name}");
                    updateStatus?.Invoke($"Added Zone: {mapZone.Name}");
                }
            }

            async Task PopulatePathGuides()
            {
                logger.Info("Populating Path Guides Detail");
                updateStatus?.Invoke("Populating Path Guides Detail");
                List<PathGuideApiSchema.GetPathGuidesByMapSnapshot>? pathGuidesByMapSnapshots =
                    await PathGuideApi.GetPathGuidesByMap(caller, guid);

                if (pathGuidesByMapSnapshots == null)
                {
                    logger.Info("No path guides found in map");
                    updateStatus?.Invoke("No path guides found in map");
                    return;
                }

                ConcurrentDictionary<string, Map.PathGuide> pathGuideList =
                    new ConcurrentDictionary<string, Map.PathGuide>();
                List<Task> getPathGuideInfoTasks = new List<Task>();
                foreach (var pathGuide in pathGuidesByMapSnapshots)
                {
                    Thread.Sleep(100);
                    Task populateBasicPathGuideDetail = PopulateBasicPathGuideDetail();
                    Task populatePathGuidePositions = PopulatePathGuidePositions();
                    getPathGuideInfoTasks.Add(populateBasicPathGuideDetail);
                    getPathGuideInfoTasks.Add(populatePathGuidePositions);

                    continue;

                    async Task PopulateBasicPathGuideDetail()
                    {
                        PathGuideApiSchema.GetPathGuideByGuidSnapshot
                            pathGuideSnapshot = await PathGuideApi.GetPathGuideByGuid(caller, pathGuide.Guid) ??
                                                throw new InvalidOperationException("Path Guide not found in robot");
                        Map.PathGuide mapPathGuide = new Map.PathGuide();
                        mapPathGuide.SetPathGuideData(
                            pathGuideSnapshot.Guid,
                            pathGuideSnapshot.Name
                        );
                        if (!pathGuideList.TryAdd(pathGuideSnapshot.Guid, mapPathGuide))
                        {
                            pathGuideList[pathGuideSnapshot.Guid].SetPathGuideData(
                                pathGuideSnapshot.Guid,
                                pathGuideSnapshot.Name
                            );
                        }
                    }

                    async Task PopulatePathGuidePositions()
                    {
                        ConcurrentBag<Map.PathGuide.PathPosition> pathGuidePositions =
                            new ConcurrentBag<Map.PathGuide.PathPosition>();
                        logger.Info("Populating Path Guide Positions Detail");
                        updateStatus?.Invoke("Populating Path Guide Positions Detail");
                        List<String>? guidList = await PositionApi.GetPositionsByPathGuide(caller, pathGuide.Guid);
                        if (guidList == null)
                        {
                            logger.Info("No positions found in path guide");
                            updateStatus?.Invoke("No positions found in path guide");
                            return;
                        }

                        List<Task> getPathGuidePositionsInfoTasks = new List<Task>();
                        foreach (var pathPositionGuid in guidList)
                        {
                            getPathGuidePositionsInfoTasks.Add(PopulatePathGuidePositionDetail(pathPositionGuid));
                        }

                        await Task.WhenAll(getPathGuidePositionsInfoTasks);
                        Map.PathGuide mapPathGuide = new Map.PathGuide();
                        mapPathGuide.PathPositions = pathGuidePositions.ToArray();

                        if (!pathGuideList.TryAdd(pathGuide.Guid, mapPathGuide))
                        {
                            pathGuideList[pathGuide.Guid].PathPositions = pathGuidePositions.ToArray();
                        }

                        return;

                        async Task PopulatePathGuidePositionDetail(string pathPositionGuid)
                        {
                            PositionApiSchema.GetPositionByPathGuideSnapshot pathPositionSnapshot =
                                await PositionApi.GetPositionByPathGuide(caller, pathGuide.Guid, pathPositionGuid);
                            Map.PathGuide.PathPosition mapPathPosition = new Map.PathGuide.PathPosition();
                            mapPathPosition.SetPathPositionData(
                                pathPositionSnapshot.Guid,
                                pathPositionSnapshot.PositionsGuid,
                                pathPositionSnapshot.Priority,
                                pathPositionSnapshot.PositionType
                            );
                            pathGuidePositions.Add(mapPathPosition);
                        }
                    }
                }

                await Task.WhenAll(getPathGuideInfoTasks);
                map.PathGuides = pathGuideList.Values.ToArray();
                logger.Info("Finished Populating Path Guides Detail");
                updateStatus?.Invoke("Finished Populating Path Guides Detail");
            }
        }
        catch (AuthenticationException e)
        {
            logger.Error("Failed to authenticate with robot in export map", e);
            updateStatus?.Invoke("Failed to authenticate with robot in export map");
            return new Map();
        }
        catch (Exception ex)
        {
            logger.Error("Failed to export map", ex);
            updateStatus?.Invoke("Failed to export map");
            return new Map();
        }
    }

    public static async Task RemoveMap(RobotSchema.Robot robot, Map map, Action<string>? updateStatus = null)
    {
        //This function purges all Map reference from the robot
        //Please backup the Map before performing this function
        try
        {
            logger.Info("Setting up API Caller");
            updateStatus?.Invoke("Setting up API Caller");
            ApiCaller caller = new ApiCaller(robot.Ip, robot.AuthId);
            try
            {
                logger.Info("Removing Map");
                updateStatus?.Invoke("Removing Map");
                MapApiSchema.GetMapByGuidSnapshot? mapSnapshot = await MapApi.GetMapByGuid(caller, map.Guid);
                if (mapSnapshot == null)
                {
                    logger.Info("Map not found in robot");
                    updateStatus?.Invoke("Map not found in robot");
                    throw new InvalidOperationException("Map not found in robot");
                }

                await MapApi.DeleteMap(caller, map.Guid);
                logger.Info("Completed Removing Map");
                updateStatus?.Invoke("Completed Removing Map");
            }
            catch (InvalidOperationException)
            {
            }
            catch (Exception e)
            {
                logger.Info("Failed to Removing Map");
                updateStatus?.Invoke("Failed to Removing Map");
                return;
            }

            foreach (Map.Zone zone in map.Zones)
            {
                try
                {
                    logger.Info($"Removing Zone: {zone.Name}");
                    updateStatus?.Invoke($"Removing Zone: {zone.Name}");
                    ZoneApiSchema.GetZoneByGuidSnaphot? zoneSnapshot = await ZoneApi.GetZoneByGuid(caller, zone.Guid);
                    if (zoneSnapshot == null)
                    {
                        logger.Info($"Zone: {zone.Name} not found in robot");
                        updateStatus?.Invoke($"Zone: {zone.Name} not found in robot");
                        continue;
                    }

                    await ZoneApi.DeleteZone(caller, zone.Guid);
                    logger.Info($"Completed Removing Zone: {zone.Name}");
                    updateStatus?.Invoke($"Completed Removing Zone: {zone.Name}");
                }
                catch (Exception e)
                {
                    logger.Info($"Failed to Removing Zone: {zone.Name}");
                    updateStatus?.Invoke($"Failed to Removing Zone: {zone.Name}");
                    return;
                }
            }

            foreach (Map.PathGuide pathGuide in map.PathGuides)
            {
                try
                {
                    logger.Info($"Removing Path Guide: {pathGuide.Name}");
                    updateStatus?.Invoke($"Removing Path Guide: {pathGuide.Name}");
                    PathGuideApiSchema.GetPathGuideByGuidSnapshot? pathGuideSnapshot =
                        await PathGuideApi.GetPathGuideByGuid(caller, pathGuide.Guid);
                    if (pathGuideSnapshot == null)
                    {
                        logger.Info($"Path Guide: {pathGuide.Name} not found in robot");
                        updateStatus?.Invoke($"Path Guide: {pathGuide.Name} not found in robot");
                        continue;
                    }

                    await PathGuideApi.DeletePathGuide(caller, pathGuide.Guid);
                    logger.Info($"Completed Removing Path Guide: {pathGuide.Name}");
                    updateStatus?.Invoke($"Completed Removing Path Guide: {pathGuide.Name}");
                }
                catch (Exception e)
                {
                    logger.Info($"Failed to Removing Path Guide: {pathGuide.Name}");
                    updateStatus?.Invoke($"Failed to Removing Path Guide: {pathGuide.Name}");
                    return;
                }
            }

            foreach (Map.Position position in map.Positions)
            {
                try
                {
                    logger.Info($"Removing Position: {position.Name}");
                    updateStatus?.Invoke($"Removing Position: {position.Name}");
                    PositionApiSchema.GetPositionByGuidSnapshot? positionSnapshot =
                        await PositionApi.GetPositionByGuid(caller, position.Guid);
                    if (positionSnapshot == null)
                    {
                        logger.Info($"Position: {position.Name} not found in robot");
                        updateStatus?.Invoke($"Position: {position.Name} not found in robot");
                        continue;
                    }

                    await PositionApi.DeletePosition(caller, position.Guid);
                    logger.Info($"Completed Removing Position: {position.Name}");
                    updateStatus?.Invoke($"Completed Removing Position: {position.Name}");
                }
                catch (Exception e)
                {
                    logger.Info($"Failed to Removing Position: {position.Name}");
                    updateStatus?.Invoke($"Failed to Removing Position: {position.Name}");
                    return;
                }
            }
        }
        catch (AuthenticationException e)
        {
            logger.Error("Failed to authenticate with robot in remove map", e);
            updateStatus?.Invoke("Failed to authenticate with robot in remove map");
            return;
        }
        catch (Exception ex)
        {
            logger.Error("Failed to remove map", ex);
            updateStatus?.Invoke("Failed to remove map");
            return;
        }
    }


    public static async Task ImportMap(RobotSchema.Robot robot, Map map, Action<string>? updateStatus = null)
    {
        //Import a single map to the robot
        //This function assumes that all object with the same name has been removed 
        
        //Create and populate basic map detail;
        try
        {
            logger.Info("Setting up API Caller");
            updateStatus?.Invoke("Setting up API Caller");
            ApiCaller caller = new ApiCaller(robot.Ip, robot.AuthId);
            logger.Info("Starting Import");
            updateStatus?.Invoke("Starting Import");
            MapApiSchema.GetMapByGuidSnapshot? mapSnapshot = await MapApi.GetMapByGuid(caller, map.Guid);
            if (mapSnapshot != null)
            {
                logger.Info("Map already exists in robot");
                updateStatus?.Invoke("Map already exists in robot");
                throw new InvalidOperationException("Map already exists in robot");
            }

            logger.Info("Importing Map Detail");
            updateStatus?.Invoke("Importing Map Detail");
            await MapApi.PostMap(caller, map.Guid, map.Name, map.GetMapBase64() , map.SiteId, map.OriginX, map.OriginY, map.OriginTheta , map.Resolution);
            logger.Info("Finished Importing Map Detail");
            updateStatus?.Invoke("Finished Importing Map Detail");

            foreach (Map.Position position in map.Positions)
            {
                try
                {
                    logger.Info($"Importing Position: {position.Name}");
                    updateStatus?.Invoke($"Importing Position: {position.Name}");
                    PositionApiSchema.GetPositionByGuidSnapshot? positionSnapshot =
                        await PositionApi.GetPositionByGuid(caller, position.Guid);
                    if (positionSnapshot != null)
                    {
                        logger.Info($"Position: {position.Name} already exists in robot");
                        updateStatus?.Invoke($"Position: {position.Name} already exists in robot");
                        continue;
                    }

                    // await PositionApi.PostPosition(caller, position.Guid, position.Name, position.PosX, position.PosY,
                    //     position.Orientation, position.TypeId);
                    logger.Info($"Finished Importing Position: {position.Name}");
                    updateStatus?.Invoke($"Finished Importing Position: {position.Name}");
                }
                catch (Exception e)
                {
                    logger.Info($"Failed to Import Position: {position.Name}");
                    updateStatus?.Invoke($"Failed to Import Position: {position.Name}");
                    return;
                }
            }

            foreach (Map.Zone zone in map.Zones)
            {
                try
                {
                    logger.Info($"Importing Zone: {zone.Name}");
                    updateStatus?.Invoke($"Importing Zone: {zone.Name}");
                    ZoneApiSchema.GetZoneByGuidSnaphot? zoneSnapshot = await ZoneApi.GetZoneByGuid(caller, zone.Guid);
                }
                catch (Exception e)
                {
                    
                }
            }
        }
        catch (Exception e)
        {
                    
        }


        return;
    }
}