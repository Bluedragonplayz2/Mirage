using System.Diagnostics;

namespace Mir_Utilities.Common;

public class StatusObject
{
    public class TaskCompleteReport(TaskCompleteReport.TaskStatus taskStatus)
    {
        public enum TaskStatus
        {
            COMPLETED = 0,
            PARTIALFAILURE = 1,
            FAILED = 2
        }

        public List<Failure>? FailureObjects = new();

        public TaskStatus Status = taskStatus;
        public string StatusMessage = "";

        public void Fatal(string? msg, string robotName, string failureMessage, Exception? exception,
            StackTrace? stackTrace)
        {
            StatusMessage = msg ?? "Fatal error occurred";
            Status = TaskStatus.FAILED;
            var failure = new Failure
                { RobotName = robotName, Message = failureMessage, Exception = exception, StackTrace = stackTrace };
            FailureObjects.Add(failure);
        }

        public void PartialFailure(string robotName, string msg, Exception? exception, StackTrace? stackTrace)
        {
            Status = TaskStatus.PARTIALFAILURE;
            FailureObjects.Add(new Failure
                { RobotName = robotName, Message = msg, Exception = exception, StackTrace = stackTrace });
        }

        public struct Failure
        {
            public string RobotName;
            public string Message;
            public Exception? Exception;
            public StackTrace? StackTrace;
        }
    }

    public class TaskInterimEvent : EventArgs
    {
        public enum EventLevel
        {
            INFO = 0,
            WARNING = 1,
            ERROR = 2
        }

        public Exception? Exception;
        public EventLevel Level;
        public string Message;
        public string? RobotName;
        public StackTrace? StackTrace;

        public TaskInterimEvent(EventLevel level, string message, string robotName, Exception exception,
            StackTrace stackTrace)
        {
            Level = level;
            Message = message;
            RobotName = robotName;
            Exception = exception;
            StackTrace = stackTrace;
        }
        public TaskInterimEvent(EventLevel level, string message, string robotName)
        {
            Level = level;
            Message = message;
            RobotName = robotName;
        }
        public TaskInterimEvent(EventLevel level, string message)
        {
            Level = level;
            Message = message;
        }
        public TaskInterimEvent(EventLevel level, string message, Exception exception,
            StackTrace stackTrace)
        {
            Level = level;
            Message = message;
            Exception = exception;
            StackTrace = stackTrace;
        }
    }
}