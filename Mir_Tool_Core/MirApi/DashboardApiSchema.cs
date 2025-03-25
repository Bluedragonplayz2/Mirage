namespace Mir_Utilities.MirApi;
public class DashboardApiSchema
{
    public struct GetDashboardSnapshot
    {
        public string Name;
        public string Guid;
    }

    public struct GetDashboardByGuidSnapshot
    {
        public string Guid;
        public string Name;
        public bool FleetDashboard;
        public string CreatedById;
    }

    public struct GetWidgetsByDashboardSnapshot
    {
        public string Guid;

    }
    public struct GetWidgetByGuidSnapshot
    {
        public string Guid;
        public string DashboardId;
        public string Settings;
        public string CreatedById;
    }

}