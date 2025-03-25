using Mir_Utilities.Common;
using Mir_Utilities.MirApi;
using Mir_Utilities.ToolFunction;
using Newtonsoft.Json;

namespace Console_Input.Commands;

public class Experimental
{
    public void CommandHandler(List<string> arguments, List<string> flags, List<string> options)
    {
        Console.WriteLine("This is the Experimental command.");
        string sourceRobot = arguments[0];
        
        string siteName = arguments[1];
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
        
        Map map = HandleMap.ExportMap(sourceRobotInformation, siteName).Result;
        string mapString = JsonConvert.SerializeObject(map);
        Console.WriteLine(mapString);
    }
}