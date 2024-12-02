namespace Mir_Utilities;

public class PositionApiSchema
{
    public struct GetPositionsByMapSnapshot
    {
        public String Guid;
        public String Name;
        public int TypeId;
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
    public struct GetPositionByPathGuideSnapshot
    {
        public String Guid;
        public String PositionsGuid;
        public int Priority;
        public String PositionType;
    }
}