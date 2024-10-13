namespace Mir_Utilities;

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

public struct GetZonesByMapSnapshot
{
    public String Guid;
    public String Name;
    public int TypeId;
}

public struct GetPositionsByMapSnapshot
{
    public String Guid;
    public String Name;
    public int TypeId;
}
public struct GetPathGuidesByMapSnapshot
{
    public String Guid;
    public String Name;
}

public struct GetPositionByGuidSnapshot
{
    public String Name;
    public String Guid;
    public float PosX;
    public float PosY;
    public float Orientation;
    public int TypeId;
    public String? ParentId;
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

public struct GetPathGuideByGuidSnapshot
{
    public String Guid;
    public String Name;
}

public struct GetPositionByPathGuideSnapshot
{
    public String Guid;
    public String PositionsGuid;
    public int Priority;
    public String PositionType;
}