namespace Mir_Utilities.MirApi;

public class SettingsApiSchema
{
    public struct GetSettingsSnapshot
    {
        public int Id;
        public string Name;
    }

    public struct GetSettingsByIdSnapshot
    {
        public int Id;
        public string Name;
        public string FullName;
        public string Description;
        public string Value;
        public string Default;
        public string Type;
        public string ParentName;
        public string ParentId;
        public string ParentValue;
        public int SettingsGroupId;
        public string SettingsGroup;
        public string FieldType;
        //Todo map out constraints
        public dynamic Constraints;
        public string Editable;
        public string ChildrenIds;
    }
}