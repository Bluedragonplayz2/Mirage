namespace Mir_Utilities.Common;

public class RobotInfomation
{
    /*
robot:
  <fleet name>:
    <robot name>:
      ip: <robot ip>
      port: <robot port>
      credential: <credential name>
credentials:
  <credential name>:
    username: <username>
    password: <password>
*/
    
    
    public class RobotConfigSchema
    {
        
        public class RobotDetails
        {
            public string ip = "";
            public string port = "443";
            public string credential = "default";
            
        }
        public class Credential
        {
            public string username;
            public string password;
        }

        public Dictionary<string, Dictionary<string, RobotDetails>> robot =
            new Dictionary<string, Dictionary<string, RobotDetails>>();
        public Dictionary<string, Credential> credentials = new Dictionary<string, Credential>();
        
    }
    private static readonly string _robotConfigPath = "config/robotConfig.yaml";
    public static List<RobotSchema.Robot>? GetRobotsFromFleet(string fleetName)
    {
        RobotConfigSchema? robotConfig = YamlConfig.GetConfigFromFile<RobotConfigSchema>(_robotConfigPath);
        if (robotConfig == null)
        {
            throw new FileNotFoundException();
        }
        List<RobotSchema.Robot> fleetRobots = new List<RobotSchema.Robot>();
        if (robotConfig.robot.TryGetValue(fleetName, out var fleet))
        {
            foreach (var robotDetails in fleet)
            {
                    if (robotConfig.credentials.TryGetValue(robotDetails.Value.credential, out var credential))
                    {
                        fleetRobots.Add(new RobotSchema.Robot(robotDetails.Key, robotDetails.Value.ip, robotDetails.Value.port, credential.username, credential.password));
                    }
                
            }
        }

        if (fleetRobots.Count == 0)
        {
            return null;
        }
        return fleetRobots;
    }
    public static List<RobotSchema.Robot>? GetAllRobots()
    {
        RobotConfigSchema? robotConfig = YamlConfig.GetConfigFromFile<RobotConfigSchema>(_robotConfigPath);
        if (robotConfig == null)
        {
            return null;
        }
        List<RobotSchema.Robot> allRobots = new List<RobotSchema.Robot>();
        foreach (var fleet in robotConfig.robot.Values)
        {
            foreach (var robot in fleet)
            {

                    if (robotConfig.credentials.TryGetValue(robot.Value.credential, out var credential))
                    {
                        allRobots.Add(new RobotSchema.Robot(robot.Key, robot.Value.ip, robot.Value.port, credential.username, credential.password));
                    }
                
            }
        }
        if (allRobots.Count == 0)
        {
            return null;
        }
        return allRobots;
    }
    
    #warning This feature is under development.
    public static RobotSchema.Robot? GetRobot(string fleetName, string name)
    {
        RobotConfigSchema? robotConfig = YamlConfig.GetConfigFromFile<RobotConfigSchema>(_robotConfigPath);
        if (robotConfig == null)
        {
            return null;
        }
        if (robotConfig.robot.TryGetValue(fleetName, out var fleet))
        {
            if (fleet.TryGetValue(name, out var robotDetails))
            {
                if (robotConfig.credentials.TryGetValue(robotDetails.credential, out var credential))
                {
                    return new RobotSchema.Robot(name, robotDetails.ip, robotDetails.port, credential.username, credential.password);
                }
            }
        }

        return null; 
    }
}