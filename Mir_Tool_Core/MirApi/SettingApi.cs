namespace Mir_Utilities.MirApi;

public class SettingApi
{
    public static async Task<List<SettingsApiSchema.GetSettingsSnapshot>?> GetSettings(ApiCaller caller)
    {
        dynamic response = await caller.GetApi("settings");
        if (response.Count == 0)
        {
            return null;
        }
        List<SettingsApiSchema.GetSettingsSnapshot> settingListSnapshot = new List<SettingsApiSchema.GetSettingsSnapshot>();
        foreach (var setting in response)
        {
            SettingsApiSchema.GetSettingsSnapshot settingSnapshot = new SettingsApiSchema.GetSettingsSnapshot();
            settingSnapshot.Name = (setting.name != null) ? setting.name : "Unnamed Setting";
            settingSnapshot.Id = (setting.guid != null) ? setting.id : "Unknown ID";
            settingListSnapshot.Add(settingSnapshot);
        }

        return settingListSnapshot;

    }

    public static async void GetSettingById(ApiCaller caller, string id)
    {
        dynamic response = await caller.GetApi($"settings/{id}");
        
    }

    public static async void PutSettingById(ApiCaller caller, string id, string value)
    {
        
    }
}