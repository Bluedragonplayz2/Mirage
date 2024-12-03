namespace Mir_Utilities;

public class FootprintApi
{
    public async Task<List<FootprintApiSchema.GetFootprintsSnapshot>?> GetFootprints(ApiCaller caller)
    {
        dynamic footprintList = await caller.GetApi($"footprints");
        if (footprintList.Count == 0)
        {
            return null;
        }

        List<FootprintApiSchema.GetFootprintsSnapshot> footprintListSnapshot = new List<FootprintApiSchema.GetFootprintsSnapshot>();

        foreach (var footprint in footprintList)
        {
            FootprintApiSchema.GetFootprintsSnapshot footprintSnapshot = new FootprintApiSchema.GetFootprintsSnapshot();
            footprintSnapshot.Name = footprint.name!;
            footprintSnapshot.Guid = footprint.guid!;
            footprintSnapshot.ConfigId = footprint.config_id!;
            footprintListSnapshot.Add(footprintSnapshot);
        }
        return footprintListSnapshot;
    }
    public async Task<FootprintApiSchema.GetFootprintByGuidSnapshot> GetFootprintByGuid(ApiCaller caller, String guid)
    {
        dynamic footprintApi = await caller.GetApi($"footprints/{guid}");
        FootprintApiSchema.GetFootprintByGuidSnapshot footprintSnapshot = new FootprintApiSchema.GetFootprintByGuidSnapshot();
        footprintSnapshot.Name = footprintApi.name!;
        footprintSnapshot.Guid = footprintApi.guid!;
        footprintSnapshot.ConfigId = footprintApi.config_id!;
        footprintSnapshot.Height = footprintApi.height!;
        footprintSnapshot.OwnerId = footprintApi.owner_id!;
        footprintSnapshot.FootprintPoints = new List<FootprintApiSchema.GetFootprintByGuidSnapshot.Coordinates>();

        foreach (var coord in footprintApi.footprint_points!)
        {
            FootprintApiSchema.GetFootprintByGuidSnapshot.Coordinates coordinates = new FootprintApiSchema.GetFootprintByGuidSnapshot.Coordinates();
            coordinates.X = coord[0];
            coordinates.Y = coord[1];
            footprintSnapshot.FootprintPoints.Add(coordinates);
        }
        return footprintSnapshot;
    }
    public async Task<String> PostFootprint(ApiCaller caller, String guid, String name, String configId,float height, List<FootprintApiSchema.GetFootprintByGuidSnapshot.Coordinates> footprintPoints)
    {
        dynamic footprint = new
        {
            guid,
            name,
            config_id = configId,
            height,
            footprint_points = footprintPoints.Select(x => new List<float> { x.X, x.Y }).ToList()
        };
        dynamic response = await caller.PostApi("footprints", footprint);
        return response.guid!;
    }
    public async Task<String> PutFootprint(ApiCaller caller, String guid, String name, String configId, float height, List<FootprintApiSchema.GetFootprintByGuidSnapshot.Coordinates> footprintPoints)
    {
        dynamic footprint = new
        {
            name,
            config_id = configId,
            height,
            footprint_points = footprintPoints.Select(x => new List<float> { x.X, x.Y }).ToList()
        };
        dynamic response = await caller.PutApi($"footprints/{guid}", footprint);
        return response.guid!;
    }
    public void DeleteFootprint(ApiCaller caller, String guid)
    {
        caller.DeleteApi($"footprints/{guid}");
    }
}