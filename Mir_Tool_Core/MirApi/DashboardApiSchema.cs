namespace Mir_Utilities;

public class DashboardApiSchema
{
    public struct GetDashboardSnapshot
    {
        public String Name;
        public String Guid;
    }

    public struct GetDashboardByGuidSnapshot
    {
        public String Guid;
        public String Name;
        public Boolean FleetDashboard;
        public String CreatedById;
    }

    public struct GetWidgetsByDashboardSnapshot
    {
        public String Guid;

    }
    public struct GetWidgetByGuidSnapshot
    {
        public String Guid;
        public String DashboardId;
        public String Settings;
        public String CreatedById;
    }

}