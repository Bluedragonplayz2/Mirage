using Mir_Utilities.Common;

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
    public static void CommandHandler(List<string> arguments, List<string> flags, List<string> options)
    {
        string sourceRobot = arguments[0];
        string[] targetRobots = arguments[1].Split(",");
        string siteName = arguments[2];
        if (sourceRobot.Contains("/"))
        {
            string[] t = sourceRobot.Split("/");
            RobotSchema.Robot? sourceRobotInformation = RobotInfomation.GetRobot(t[0], t[1]);
            if (sourceRobotInformation == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Source robot not found");
                Console.ResetColor();
                return;
            }
        }
        else if(sourceRobot.Contains(":"))
        {
            string[] t = sourceRobot.Split(":");
            if (t.Length == 4)
            {
                RobotSchema.Robot sourceRobotInformation = new RobotSchema.Robot("source", t[0], t[1], t[2], t[3]);
            }
            else if (t.Length == 3)
            {
                RobotSchema.Robot sourceRobotInformation = new RobotSchema.Robot("source", t[0], "443", t[1], t[2]);
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
            else if(targetRobot.Contains(":"))
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
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid target robot format");
                Console.ResetColor();
                return;
            }
        }
        
        
        
        Mir_Utilities.SyncSite taskExec = new Mir_Utilities.SyncSite();
        
        taskExec.TaskUpdateEvent += HandleUpdateEvent;
        

    }
    private static void HandleUpdateEvent(object? sender, StatusObject.TaskInterimEvent e)
    {
        Console.WriteLine(e.Message);
    }
}