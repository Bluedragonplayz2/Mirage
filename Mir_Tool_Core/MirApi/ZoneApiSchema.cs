namespace Mir_Utilities;

public class ZoneApiSchema
{
    public struct GetZonesByMapSnapshot
    {
        public String Guid;
        public String Name;
        public int TypeId;
    }
    public struct GetZoneByGuidSnaphot
    {
        public String Guid;
        public String Name;
        public String ShapeType;
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