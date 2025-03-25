using Mir_Utilities.Common;

namespace Console_Input.Commands;

public class ClearMission
{
    /*
    Command to clear all Missions of the credentials provided from a robot
    Usage: clearmission <robot(s)>
    Alias: cm
    Arguments:
    - robot(s): The name of the robot(s) to clear footprints from
        format: [fleet name*] or [fleet name*]/[robot name*] or [ip*]:[port]:[username*]:[password*] or "all" for all robots in config, seperated by ","
    */
    private Dictionary<string, int[]> _curserPositions = new Dictionary<string, int[]>();
    private int[] _endCursorPosition = new int[2];
    private static readonly object consolePointer = new object();

    public void CommandHandler(List<string> arguments, List<string> flags, List<string> options)
    {
        string[] targetRobots = arguments[0].Split(",");
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
                    Console.WriteLine("Target robot not found from robot name");
                    _endCursorPosition = [Console.GetCursorPosition().Left, Console.GetCursorPosition().Top];   
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
                    _endCursorPosition = [Console.GetCursorPosition().Left, Console.GetCursorPosition().Top];
                    Console.ResetColor();
                    return;
                }
            }
            else
            {
                Console.WriteLine(targetRobot);
                List<RobotSchema.Robot> _targetRobots = RobotInfomation.GetRobotsFromFleet(targetRobot);
                if (_targetRobots != null)
                {
                    targetRobotsInformation.AddRange(_targetRobots);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid target robot format from fleet");
                    _endCursorPosition = [Console.GetCursorPosition().Left, Console.GetCursorPosition().Top];
                    Console.ResetColor();
                    return;
                }
            }
        }

        foreach (var robot in targetRobotsInformation)
        {
            try
            {
                Mir_Utilities.ClearMission.ClearMissionFromRobot(robot);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Failed to clear footprint from {robot.Name}");
                Console.WriteLine(e.Message);
                _endCursorPosition = [Console.GetCursorPosition().Left, Console.GetCursorPosition().Top];
                Console.ResetColor();
            }
        }
        
        
        Console.WriteLine($"Completed");
        

    }
}