using Mir_Utilities.Common;
using YamlDotNet.Core;

namespace Console_Input.Commands;

using Mir_Utilities;

public class SyncSite
{
    /*
    Command to transfer site files from one target robot to multiple other robots
    Usage: syncsite <source robot> <target robots> <site name>
    Alias: ss
    Arguments:
    - <source robot> : The robot that contains the site to be transferred
        format: [fleet name*]/[robot name*] or [ip*]:[port]:[username*]:[password*]
    - <target robots> : The robots that will receive the site
        format: [fleet name*] or [fleet name*]/[robot name*] or [ip*]:[port]:[username*]:[password*] or "all" for all robots in config, seperated by ","
    - <site name> : The name of the site to be transferred


    */
    private RobotSchema.Robot _sourceRobot;
    private List<RobotSchema.Robot> _targetRobots;
    private string _siteName;
    private Dictionary<string, int[]> _curserPositions = new Dictionary<string, int[]>();

    public void CommandHandler(List<string> arguments, List<string> flags, List<string> options)
    {
        string sourceRobot = arguments[0];
        string[] targetRobots = arguments[1].Split(",");
        string siteName = arguments[2];
        RobotSchema.Robot? sourceRobotInformation;
        if (sourceRobot.Contains("/"))
        {
            string[] t = sourceRobot.Split("/");
            sourceRobotInformation = RobotInfomation.GetRobot(t[0], t[1]);
            if (sourceRobotInformation == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Source robot not found");
                Console.ResetColor();
                return;
            }
        }
        else if (sourceRobot.Contains(":"))
        {
            string[] t = sourceRobot.Split(":");
            if (t.Length == 4)
            {
                sourceRobotInformation = new RobotSchema.Robot("source", t[0], t[1], t[2], t[3]);
            }
            else if (t.Length == 3)
            {
                sourceRobotInformation = new RobotSchema.Robot("source", t[0], "443", t[1], t[2]);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid source robot format");
                Console.ResetColor();
                return;
            }
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid source robot format");
            Console.ResetColor();
            return;
        }

        List<RobotSchema.Robot> targetRobotsInformation = new List<RobotSchema.Robot>();
        foreach (string targetRobot in targetRobots)
        {
            if (targetRobot == "all")
            {
                List<RobotSchema.Robot> allRobots = RobotInfomation.GetAllRobots();
                targetRobotsInformation.AddRange(allRobots);
            }
            else if (targetRobot.Contains("/"))
            {
                string[] t = targetRobot.Split("/");
                RobotSchema.Robot? targetRobotInformation = RobotInfomation.GetRobot(t[0], t[1]);
                if (targetRobotInformation == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Target robot not found");
                    Console.ResetColor();
                    return;
                }

                targetRobotsInformation.Add(targetRobotInformation);
            }
            else if (targetRobot.Contains(":"))
            {
                string[] t = targetRobot.Split(":");
                if (t.Length == 4)
                {
                    RobotSchema.Robot targetRobotInformation = new RobotSchema.Robot("target", t[0], t[1], t[2], t[3]);
                    targetRobotsInformation.Add(targetRobotInformation);
                }
                else if (t.Length == 3)
                {
                    RobotSchema.Robot targetRobotInformation = new RobotSchema.Robot("target", t[0], "443", t[1], t[2]);
                    targetRobotsInformation.Add(targetRobotInformation);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid target robot format");
                    Console.ResetColor();
                    return;
                }
            }
            else
            {
                List<RobotSchema.Robot> _targetRobots = RobotInfomation.GetRobotsFromFleet(targetRobot);
                if (_targetRobots != null)
                {
                    targetRobotsInformation.AddRange(_targetRobots);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid target robot format");
                    Console.ResetColor();
                    return;
                }
            }
        }


        Mir_Utilities.SyncSite taskExec = new Mir_Utilities.SyncSite();

        taskExec.TaskUpdateEvent += HandleUpdateEvent;
        StatusObject.TaskCompleteReport report =
            taskExec.SyncSiteData(sourceRobotInformation, targetRobotsInformation, siteName);
        if (report.Status == StatusObject.TaskCompleteReport.TaskStatus.FAILED)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Task failed");
            Console.WriteLine(report.StatusMessage);
            Console.ResetColor();
            foreach (var failure in report.FailureObjects)
            {
                Console.WriteLine(failure.Message);
                Console.WriteLine(failure.Exception);
                Console.WriteLine(failure.StackTrace);
            }
        }
        else if (report.Status == StatusObject.TaskCompleteReport.TaskStatus.COMPLETED)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Task Complete");
            Console.WriteLine(report.StatusMessage);
            Console.ResetColor();
        }
        else if (report.Status == StatusObject.TaskCompleteReport.TaskStatus.PARTIALFAILURE)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Task Complete but with issues");
            Console.ForegroundColor = ConsoleColor.Red;
            foreach (var failure in report.FailureObjects)
            {
                Console.WriteLine(failure.Message);
                Console.WriteLine(failure.Exception);
                Console.WriteLine(failure.StackTrace);
            }

            Console.ResetColor();
        }
    }

    private void HandleUpdateEvent(object? sender, StatusObject.TaskInterimEvent e)
    {
        if (e.RobotName == null)
        {
            Console.WriteLine($"{e.Message}");
            if (e.Exception != null)
            {
                Console.WriteLine(e.Exception);
                Console.WriteLine(e.StackTrace);
            }

            return;
        }

        if (!_curserPositions.ContainsKey(e.RobotName))
        {
            Console.Write(e.RobotName + ": ");
            int[] currentCursorPosition = new[] { Console.CursorLeft, Console.CursorTop };
            Console.WriteLine();
            _curserPositions.Add(e.RobotName, currentCursorPosition);
        }

        _curserPositions.TryGetValue(e.RobotName, out int[] cursorPosition);
        Console.SetCursorPosition(cursorPosition[0], cursorPosition[1]);
        int workableSpace = (int)Math.Floor(Console.WindowWidth * 0.60);
        if (workableSpace < 15)
        {
            return;
        }

        if (e.Message.StartsWith("Importing"))
        {
            string output = "Importing:[";
            int progressPercentage = int.Parse(e.Message.Split(":")[1]);
            if (progressPercentage == 0)
            {
                for (int i = 0; i < (workableSpace - 12); i++)
                {
                    output += " ";
                }

                output += "]";
                Console.Write(output);
                return;
            }

            float progress = (float)progressPercentage / 100;
            int progressLength = (int)Math.Floor(progress * (workableSpace - 12));
            int remainder = (workableSpace - 12) - progressLength;
            for (int i = 0; i < (progressLength - 1); i++)
            {
                output += "=";
            }

            output += ">";
            for (int i = 0; i < remainder; i++)
            {
                output += " ";
            }

            output += "]";

            Console.Write(output);
            return;
        }

        if (e.Message.Length > workableSpace)
        {
            string[] message = e.Message.Split(" ");
            string line = "";
            foreach (string word in message)
            {
                if (line.Length + word.Length > workableSpace)
                {
                    Console.WriteLine(line);
                    line = "";
                }

                line += word + " ";
            }

            Console.WriteLine(line);
        }
        else
        {
            Console.WriteLine(e.Message);
        }
    }
}