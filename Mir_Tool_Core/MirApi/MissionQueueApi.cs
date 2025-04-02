using RestSharp;

namespace Mir_Utilities.MirApi;

public class MissionQueueApi
{
    public static async Task ClearMissionQueue(ApiCaller caller)
    {
        await caller.DeleteApi("mission_queue");
    }

    public static async Task<List<MissionQueueApiSchema.GetMissionQueue>> GetMissionQueues(ApiCaller caller)
    {
        dynamic snapshot = await caller.GetApi("mission_queue");
        List<MissionQueueApiSchema.GetMissionQueue> queueSnapshot = new List<MissionQueueApiSchema.GetMissionQueue>();
        foreach (var queue in snapshot)
        {
            MissionQueueApiSchema.GetMissionQueue missionQueue = new MissionQueueApiSchema.GetMissionQueue();
            missionQueue.Id = queue.id;
            missionQueue.State = queue.state;
            queueSnapshot.Add(missionQueue);
        }

        return queueSnapshot;
    } 
}