namespace Mir_Utilities.MirApi;

public class PositionApiSchema
{
    public struct GetPositionsByMapSnapshot
    {
        public string Guid;
        public string Name;
        public int TypeId;
    }
    public struct GetPositionByGuidSnapshot
    {
        public string Name;
        public string Guid;
        public float PosX;
        public float PosY;
        public float Orientation;
        public int TypeId;
        public String? ParentId;
    }
    public struct GetPositionByPathGuideSnapshot
    {
        public string Guid;
        public string PositionsGuid;
        public int Priority;
        public string PositionType;
    }
}