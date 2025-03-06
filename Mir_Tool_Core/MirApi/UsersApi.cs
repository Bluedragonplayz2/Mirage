using System.Net;
using RestSharp;

namespace Mir_Utilities.MirApi;

public class UsersApi
{
    public static async Task<List<UsersApiSchema.GetUsersSnapshot>?> GetUsers(ApiCaller caller)
    {
        dynamic response = await caller.GetApi("users");
        if (response.Count == 0)
        {
            return null;
        }
        List<UsersApiSchema.GetUsersSnapshot> users = new List<UsersApiSchema.GetUsersSnapshot>();
        foreach (var user in response)
        {
            UsersApiSchema.GetUsersSnapshot userSnapshot = new UsersApiSchema.GetUsersSnapshot();
            userSnapshot.Guid = user.guid!;
            userSnapshot.Name = user.name!;
            users.Add(userSnapshot);
        }

        return users;

    }

    public static async void PostUser()
    {
        
    }

    public static async Task<UsersApiSchema.GetUserByGuidSnapshot> GetUserbyGuid(ApiCaller caller, string guid)
    {
        dynamic response = await caller.GetApi($"users/{guid}");
        UsersApiSchema.GetUserByGuidSnapshot user = new UsersApiSchema.GetUserByGuidSnapshot();
        user.Guid = response.guid!;
        user.Name = response.name!;
        user.Email = response.email!;
        user.DashboardId = response.dashboard_id!;
        user.CreatedById = response.created_by_id!;
        user.Pincode = response.pincode!;
        user.Username = response.username!;
        user.CreatedBy = response.created_by!;
        user.UserGroupId = response.user_group_id!;
        user.UserGroup = response.user_group!;
        user.SingleDashboard = response.single_dashboard!;
        user.CreateTime = response.create_time!;
        user.UpdateTime = response.update_time!;
        
        return user;

    }

    public static async Task<string> PutUserbyGuid(
        ApiCaller caller,
        string guid,
        string username,
        string password,
        string pincode,
        string name,
        string email,
        bool singleDashboard,
        string dashboardId,
        string userGroupId
        )
    {
        dynamic user = new
        {
            username,
            password,
            pincode,
            name,
            email,
            single_dashboard = singleDashboard,
            dashboard_id = dashboardId,
            user_group_id = userGroupId
        };
        dynamic response = await caller.PutApi($"users/{guid}", user);
        return response.guid!;
    }

    public static async Task DeleteUserbyGuid(ApiCaller caller, string guid)
    {
        RestResponse response = await caller.DeleteApi($"users/{guid}");
        if (response.StatusCode != HttpStatusCode.NoContent)
        {
            throw new Exception("Failed to delete user");
        }
    }
    
}