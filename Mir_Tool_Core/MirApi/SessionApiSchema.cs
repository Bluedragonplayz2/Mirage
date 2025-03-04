namespace Mir_Utilities;

public class SessionApiSchema
{
    public struct GetSessionSnapshot
    {
        public string Name;
        public string Guid;
    }
    public struct GetSessionByGuidSnapshot
    {
        public string Name;
        public string Guid;
        public string CreatedById;
        public string Description;
    }

    public struct GetActiveSessionImportSnapshot
    {
        //DO NOT USE, NOT TESTED 
        public int Status;
        public int SessionsTotal;
        public int SessionsImported;
        public string ErrorMessage;
    }
}