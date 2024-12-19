namespace Mir_Utilities;

public class RobotInfomation
{
    private class RobotConfigSchema
    {
        public class RobotDetails
        {
            public string ip;
            public string port;
            public string credential;
            
        }
        public class Credential
        {
            public string username;
            public string password;
        }

        public List<Dictionary<string, Dictionary<string, RobotDetails>>> robot;
        public Dictionary<string, Credential> credential;
        
    }
    private static readonly string _robotConfigPath = "./config/robotConfig.json";
    public static List<RobotSchema.Robot> GetRobotsFromFleet(string fleetName)
    {
        RobotConfigSchema? robotConfig = YamlConfig.GetConfigFromFile<RobotConfigSchema>(_robotConfigPath);
        if (robotConfig == null)
        {
            return new List<RobotSchema.Robot>();
        }
        List<RobotSchema.Robot> fleetRobots = new List<RobotSchema.Robot>();
        foreach (var fleet in robotConfig.robot)
        {
           
            if (fleet.TryGetValue(fleetName, out var robotDetails))
            {
                foreach (var robot in robotDetails)
                {

                    if (robotConfig.credential.TryGetValue(robot.Value.credential, out var credential))
                    {
                        fleetRobots.Add(new RobotSchema.Robot(robot.Key, robot.Value.ip, robot.Value.port, credential.username, credential.password));
                    }
                }
            }
        }

        return fleetRobots;
    }
    
    #warning This feature is under development.
    public static RobotSchema GetRobot(string name)
    {
        return new RobotSchema();
    }
}