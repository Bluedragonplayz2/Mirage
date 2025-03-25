namespace Mir_Utilities.MirApi;

public class ZoneApiSchema
{
    public struct GetZonesByMapSnapshot
    {
        public string Guid;
        public string Name;
        public int TypeId;
    }
    public struct GetZoneByGuidSnaphot
    {
        public string Guid;
        public string Name;
        public string ShapeType;
        public int TypeId;
        public float StrokeWidth;
        public float Direction;
        public List<Coordinates> Polygon;

        public struct Coordinates
        {
            public float X;
            public float Y;
        }
        public dynamic Actions;
    }

}