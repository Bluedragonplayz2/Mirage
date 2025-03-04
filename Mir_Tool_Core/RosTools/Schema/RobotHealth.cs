namespace Mir_Utilities.RosTools.Schema;

public class RobotHealth
{
    public DateTime recordTime;
    public List<StatusObject> status;

    public class StatusObject
    {
        public required string name;
        public string message = "";
        public List<StatusObject>? children;
        public EquipmentHealthLevel health;
        public Dictionary<string, string> values;
    }

    public enum EquipmentHealthLevel
    {
        INACTIVE = -1,
        NORMAL = 0,
        FAILED = 1,
        UNKNOWN = 2
        
    }
}