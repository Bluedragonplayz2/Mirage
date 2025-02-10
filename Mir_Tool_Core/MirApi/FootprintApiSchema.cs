namespace Mir_Utilities;

public class FootprintApiSchema
{
    public struct GetFootprintsSnapshot
    {
        public string Guid;
        public string Name;
        public string ConfigId;
    }
    public struct GetFootprintByGuidSnapshot
    {
        public string Guid;
        public string Name;
        public float Height;
        public string OwnerId;
        public string ConfigId;
        public List<Coordinates> FootprintPoints;

        public struct Coordinates
        {
            public float X;
            public float Y;
        }
    }
}