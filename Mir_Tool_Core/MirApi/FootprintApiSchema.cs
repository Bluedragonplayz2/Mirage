namespace Mir_Utilities;

public class FootprintApiSchema
{
    public struct GetFootprintsSnapshot
    {
        public String Guid;
        public String Name;
        public String ConfigId;
    }
    public struct GetFootprintByGuidSnapshot
    {
        public String Guid;
        public String Name;
        public float Height;
        public String OwnerId;
        public String ConfigId;
        public List<Coordinates> FootprintPoints;

        public struct Coordinates
        {
            public float X;
            public float Y;
        }
    }
}