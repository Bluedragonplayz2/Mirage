namespace Mir_Utilities.MirApi;
public class MapApiSchema
{
    public struct GetMapSnapshot
    {
        public string Name;
        public string Guid;
    }

    public struct GetMapByGuidSnapshot
    {
        public string BaseMap;
        public string SiteId;
        public string Name;
        public string Guid;
        public float OriginX;
        public float OriginY;
        public float OriginTheta;
        public float Resolution;
    }
}