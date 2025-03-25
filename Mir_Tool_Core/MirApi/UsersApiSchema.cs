namespace Mir_Utilities.MirApi;

public class UsersApiSchema
{
    public struct GetUsersSnapshot
    {
        public string Guid;
        public string Name;
    }
    public struct GetUserByGuidSnapshot
    {
        public string Guid;
        public string Name;
        public string Username;
        public string Email;
        public bool SingleDashboard;
        public string DashboardId;
        public string CreateTime;
        public string UpdateTime;
        public string UserGroupId;
        public string UserGroup;
        public string CreatedById;
        public string CreatedBy;
        public string Pincode;
    }
    
}