namespace Mir_Utilities;

public class MapApiSchema
{
    public struct GetMapSnapshot
    {
        public String Name;
        public String Guid;
    }

    public struct GetMapByGuidSnapshot
    {
        public String BaseMap;
        public String SiteId;
        public String Name;
        public String Guid;
        public float OriginX;
        public float OriginY;
        public float OriginTheta;
        public float Resolution;
    }
}