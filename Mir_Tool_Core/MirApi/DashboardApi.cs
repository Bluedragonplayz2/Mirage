namespace Mir_Utilities.MirApi;
public class DashboardApi
{
    public static async Task<List<DashboardApiSchema.GetDashboardSnapshot>?> GetDashboards(ApiCaller caller)
    {
        dynamic dashboardList = await caller.GetApi($"dashboards");
        if (dashboardList.Count == 0)
        {
            return null;
        }

        List<DashboardApiSchema.GetDashboardSnapshot> dashboardListSnapshot = new List<DashboardApiSchema.GetDashboardSnapshot>();

        foreach (var dashboard in dashboardList)
        {
            DashboardApiSchema.GetDashboardSnapshot dashboardSnapshot = new DashboardApiSchema.GetDashboardSnapshot();
            dashboardSnapshot.Name = dashboard.name!;
            dashboardSnapshot.Guid = dashboard.guid!;
            dashboardListSnapshot.Add(dashboardSnapshot);
        }
        return dashboardListSnapshot;
    }
    public static async Task<DashboardApiSchema.GetDashboardByGuidSnapshot> GetDashboardByGuid(ApiCaller caller, String guid)
    {
        dynamic dashboard = await caller.GetApi($"dashboards/{guid}");
        DashboardApiSchema.GetDashboardByGuidSnapshot dashboardSnapshot = new DashboardApiSchema.GetDashboardByGuidSnapshot();
        dashboardSnapshot.Name = dashboard.name!;
        dashboardSnapshot.Guid = dashboard.guid!;
        dashboardSnapshot.FleetDashboard = dashboard.fleet_dashboard!;
        dashboardSnapshot.CreatedById = dashboard.created_by_id!;
        return dashboardSnapshot;
    }
    public static async Task<String> PostDashboard(ApiCaller caller, String guid, String name, String fleetDashboard)
    {
        dynamic dashboard = new
        {
            guid,
            name,
            fleet_dashboard = fleetDashboard,
        };
        dynamic response = await caller.PostApi("dashboards", dashboard);
        return response.guid!;
    }
    public static async Task<String> PutDashboard(ApiCaller caller, String guid, String name, String fleetDashboard)
    {
        dynamic dashboard = new
        {
            name,
            fleet_dashboard = fleetDashboard,
        };
        dynamic response = await caller.PutApi($"dashboards/{guid}", dashboard);
        return response.guid!;
    }
    public static void DeleteDashboard(ApiCaller caller, String guid)
    {
        caller.DeleteApi($"dashboards/{guid}");
    }
    public static async Task<List<DashboardApiSchema.GetWidgetsByDashboardSnapshot>?> GetWidgetsByDashboard(ApiCaller caller, String dashboardGuid)
    {
        dynamic widgetList = await caller.GetApi($"dashboards/{dashboardGuid}/widgets");
        if (widgetList.Count == 0)
        {
            return null;
        }

        List<DashboardApiSchema.GetWidgetsByDashboardSnapshot> widgetListSnapshot = new List<DashboardApiSchema.GetWidgetsByDashboardSnapshot>();

        foreach (var widget in widgetList)
        {
            DashboardApiSchema.GetWidgetsByDashboardSnapshot widgetSnapshot = new DashboardApiSchema.GetWidgetsByDashboardSnapshot();
            widgetSnapshot.Guid = widget.guid!;
            widgetListSnapshot.Add(widgetSnapshot);
        }

        return widgetListSnapshot;
        
    }
    public static async Task<DashboardApiSchema.GetWidgetByGuidSnapshot> GetWidgetByGuid(ApiCaller caller, String dashboardGuid, String widgetGuid)
    {
        dynamic widget = await caller.GetApi($"dashboards/{dashboardGuid}/widgets/{widgetGuid}");
        DashboardApiSchema.GetWidgetByGuidSnapshot widgetSnapshot = new DashboardApiSchema.GetWidgetByGuidSnapshot();

        widgetSnapshot.Guid = widget.guid!;
        widgetSnapshot.DashboardId = widget.dashboard_id!;
        widgetSnapshot.Settings = widget.settings!;
        widgetSnapshot.CreatedById = widget.created_by_id!;
        return widgetSnapshot;
    }
    public static async Task<String> PostWidget(ApiCaller caller, String guid, String dashboardId, String settings)
    {
        dynamic widget = new
        {
            guid,
            settings
        };
        dynamic response = await caller.PostApi($"dashboard/{dashboardId}/widgets", widget);
        return response.guid!;
    }
    public static async Task<String> PutWidget(ApiCaller caller, String guid, String dashboardId, String settings)
    {
        dynamic widget = new
        {
            settings
        };
        dynamic response = await caller.PutApi($"dashboard/{dashboardId}/widgets/{guid}", widget);
        return response.guid!;
    }
    public static void DeleteWidget(ApiCaller caller, String dashboardId, String guid)
    {
        caller.DeleteApi($"dashboard/{dashboardId}/widgets/{guid}");
    }
}