namespace Mir_Utilities.MirApi;
public class PathGuideApiSchema
{
    public struct GetPathGuidesByMapSnapshot
    {
        public string Guid;
        public string Name;
    }
    public struct GetPathGuideByGuidSnapshot
    {
        public string Guid;
        public string Name;
    }
}