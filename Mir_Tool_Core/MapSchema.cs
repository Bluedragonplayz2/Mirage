namespace Mir_Utilities;

public struct Map
{
    public String Guid;
    public String Name;
    public String SiteId;
    public float OriginX;
    public float OriginY;
    public float Resolution;
    public float OriginTheta;
    public List<Position>? Positions;
    public List<Zone>? Zones;
    public List<PathGuide>? PathGuides;
    public String BaseMap;

    public struct Position
    {
        public String Guid;
        public String Name;
        public float PosX;
        public float PosY;
        public float Orientation;
        public int TypeId;
        public List<Position>? HelperPositions;
    }

    public struct Zone
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

    public struct PathGuide
    {
        public String Guid;
        public String Name;
        public List<PathPosition> PathPositions;

        public struct PathPosition
        {
            public String Guid;
            public String PositionGuid;
            public int Priority;
            public String PositionType;
        }
    }
    
}



