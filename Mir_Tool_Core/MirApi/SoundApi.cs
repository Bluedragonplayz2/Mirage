namespace Mir_Utilities;

public class SoundApi
{
    public static async Task<List<SoundApiSchema.GetSoundsSnapshot>> GetSounds(ApiCaller caller)
    {
        dynamic SoundsList = await caller.GetApi("sounds");
        if (SoundsList.Count == 0)
        {
            return null;
        }
        List<SoundApiSchema.GetSoundsSnapshot> getSoundsSnapshots = new List<SoundApiSchema.GetSoundsSnapshot>();
        foreach (var sound in SoundsList)
        {
            SoundApiSchema.GetSoundsSnapshot getSoundsSnapshot = new SoundApiSchema.GetSoundsSnapshot();
            getSoundsSnapshot.Name = sound.name!;
            getSoundsSnapshot.Guid = sound.guid!;
            getSoundsSnapshots.Add(getSoundsSnapshot);
        }

        return getSoundsSnapshots;
        

    }

    public static async Task<SoundApiSchema.GetSoundByGuidSnapshot> GetSoundsByGuid(ApiCaller caller, String guid)
    {  
        dynamic sound = await caller.GetApi($"sounds/{guid}");
        SoundApiSchema.GetSoundByGuidSnapshot getSoundByGuidSnapshot = new SoundApiSchema.GetSoundByGuidSnapshot();
        getSoundByGuidSnapshot.Name = sound.name!;
        getSoundByGuidSnapshot.Guid = sound.guid!;
        getSoundByGuidSnapshot.Volume = sound.volume!;
        getSoundByGuidSnapshot.Note = sound.note!;
        getSoundByGuidSnapshot.Sound = sound.sound!;
        getSoundByGuidSnapshot.OwnerId = sound.owner_id!;
        getSoundByGuidSnapshot.Length = TimeSpan.Parse(sound.length);
        return getSoundByGuidSnapshot;
    }
    public static async Task<String> PostSound(ApiCaller caller, String guid, String name, String sound, String note, int volume)
    {
        dynamic soundObject = new
        {
            guid,
            name,
            sound,
            note,
            volume
        };
        dynamic response = await caller.PostApi("sounds", soundObject);
        return response.guid!;
    }
    public static async Task<String> PutSound(ApiCaller caller, String guid, String name, String sound, String note, int volume)
    {
        dynamic soundObject = new
        {
            name,
            sound,
            note,
            volume
        };
        dynamic response = await caller.PutApi($"sounds/{guid}", soundObject);
        return response.guid!;
    }
    public static void DeleteSound(ApiCaller caller, String guid)
    {
        caller.DeleteApi($"sounds/{guid}");
    }
    
    
}