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
}