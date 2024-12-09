namespace Mir_Utilities;

public class SessionApiSchema
{
    public struct GetSessionSnapshot
    {
        public String Name;
        public String Guid;
    }
    public struct GetSessionByGuidSnapshot
    {
        public String Name;
        public String Guid;
        public String CreatedById;
        public String Description;
    }

    public struct GetActiveSessionImportSnapshot
    {
        //DO NOT USE, NOT TESTED 
        public int Status;
        public int SessionsTotal;
        public int SessionsImported;
        public String ErrorMessage;
    }
}