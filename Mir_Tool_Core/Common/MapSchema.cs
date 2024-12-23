namespace Mir_Utilities.Common;

public struct Map
{
    public string Guid;
    public string Name;
    public string SiteId;
    public float OriginX;
    public float OriginY;
    public float Resolution;
    public float OriginTheta;
    public List<Position>? Positions;
    public List<Zone>? Zones;
    public List<PathGuide>? PathGuides;
    public string BaseMap;

    public struct Position
    {
        public string Guid;
        public string Name;
        public float PosX;
        public float PosY;
        public float Orientation;
        public int TypeId;
        public List<Position>? HelperPositions;
    }

    public struct Zone
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

    public struct PathGuide
    {
        public string Guid;
        public string Name;
        public List<PathPosition> PathPositions;

        public struct PathPosition
        {
            public string Guid;
            public string PositionGuid;
            public int Priority;
            public string PositionType;
        }
    }
    
}



