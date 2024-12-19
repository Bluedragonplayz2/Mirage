namespace Mir_Utilities;

public class UpdateEventArgs: EventArgs
{
    public enum TaskStatus
    {
        COMPLETED,
        PARTIALFAILURE,
        FAILED,
    }
    
    public TaskStatus Status;
    public String TaskId;
    public String StatusMsg;
    public List<Failure>? FailureObjects;
    public struct Failure
    {
        public Exception Exception;
        public String Message;
        public String StackTrace;
    }
    public dynamic? Object;
    
}