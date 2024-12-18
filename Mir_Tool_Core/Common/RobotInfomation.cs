namespace Mir_Utilities;

public class RobotInfomation
{
    private struct RobotConfigSchema
    {
        public struct _robotDetails
        {
            public string ip;
            public string port;
            public string credential;
            
        }
        public struct _credential
        {
            public string username;
            public string password;
        }

        public List<Dictionary<string, _robotDetails>> robot;
        public List<Dictionary<string, _credential >> credential;
        
    }
    private readonly string _robotConfigPath = "./config/robotConfig.json";
    public static List<RobotSchema.Robot> GetRobotsFromFleet(string fleetName)
    {
        
        return new List<RobotSchema.Robot>();
    }

    public static RobotSchema GetRobot(string name)
    {
        
    }
}