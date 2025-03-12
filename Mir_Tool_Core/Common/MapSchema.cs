
using System.Drawing;
using System.Runtime.InteropServices;

namespace Mir_Utilities.Common;

public class Map
{
    public string Guid { get; private set; } = "";
    public string Name { get; private set; } = "";
    public string SiteId { get; private set; } = "";
    public float OriginX { get; private set; } 
    public float OriginY { get; private set; }
    public float Resolution { get; private set; }
    public float OriginTheta { get; private set; }
    public Position[] Positions { get; set; } = [];
    public Zone[] Zones { get; set; } = [];
    public PathGuide[] PathGuides { get; set; } = [];
    public byte[] BaseMap { get; private set; } = [];
    public class Position
    {
        public string Guid { get; private set; } = "";
        public string Name { get; private set; } = "";
        public float PosX { get; private set; }
        public float PosY { get; private set; }
        public float Orientation { get; private set; }
        public int TypeId { get; private set; }
        public Position[] HelperPositions { get; set; } = [];
    }

    public class Zone
    {
        public string Guid { get; private set; } = "";
        public string Name { get; private set; } = "";
        public string ShapeType { get; private set; } = "";
        public int TypeId { get; private set; }
        public float StrokeWidth { get; private set; }
        public float Direction { get; private set; }
        public Coordinates[] Polygon { get; private set; } = [];

        public class Coordinates
        {
            public float X { get; private set; }
            public float Y { get; private set; }
        }

        public object? Actions { get; private set; } = null;

    }

    public class PathGuide
    {
        public string Guid { get; set; } = "";
        public string Name { get; set; } = "";
        public PathPosition[] PathPositions { get; set; } = [];

        public class PathPosition
        {
            public string Guid { get; set; } = "";
            public string PositionGuid { get; set; } = "";
            public int Priority { get; set; }
            public string PositionType { get; set; } = "";
        }
    }
    public void SetBasicMapData(string guid, string name, string siteId, float originX, float originY, float resolution, float originTheta, string baseMap)
    {
        Guid = guid;
        Name = name;
        SiteId = siteId;
        OriginX = originX;
        OriginY = originY;
        Resolution = resolution;
        OriginTheta = originTheta;
        BaseMap = Convert.FromBase64String(baseMap);
    }



    public string GetMapBase64()
    {
        return Convert.ToBase64String(BaseMap);
    }
    public Bitmap GetMapBitmap()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new PlatformNotSupportedException("System.Drawing.Bitmap is only supported on Windows!");
        using MemoryStream ms = new MemoryStream(BaseMap);
        return new Bitmap(ms);
    }

}



