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
    
    
    private class RobotConfigSchema
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

        public List<Dictionary<string, Dictionary<string, RobotDetails>>> robot = new List<Dictionary<string, Dictionary<string, RobotDetails>>>();
        public Dictionary<string, Credential> credentials = new Dictionary<string, Credential>();
        
    }
    private static readonly string _robotConfigPath = "./config/robotConfig.yaml";
    public static List<RobotSchema.Robot>? GetRobotsFromFleet(string fleetName)
    {
        RobotConfigSchema? robotConfig = YamlConfig.GetConfigFromFile<RobotConfigSchema>(_robotConfigPath);
        if (robotConfig == null)
        {
            return null;
        }
        List<RobotSchema.Robot> fleetRobots = new List<RobotSchema.Robot>();
        foreach (var fleet in robotConfig.robot)
        {
           
            if (fleet.TryGetValue(fleetName, out var robotDetails))
            {
                foreach (var robot in robotDetails)
                {

                    if (robotConfig.credentials.TryGetValue(robot.Value.credential, out var credential))
                    {
                        fleetRobots.Add(new RobotSchema.Robot(robot.Key, robot.Value.ip, robot.Value.port, credential.username, credential.password));
                    }
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
        foreach (var fleet in robotConfig.robot)
        {
            foreach (var robotDetails in fleet.Values)
            {
                foreach (var robot in robotDetails)
                {
                    if (robotConfig.credentials.TryGetValue(robot.Value.credential, out var credential))
                    {
                        allRobots.Add(new RobotSchema.Robot(robot.Key, robot.Value.ip, robot.Value.port, credential.username, credential.password));
                    }
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
        foreach (var fleetDict in robotConfig.robot)
        {
            // Check if the fleet dictionary contains the specified fleet name
            if (fleetDict.TryGetValue(fleetName, out var robotDetails))
            {
                // Check if the robot dictionary contains the specified robot name
                if (robotDetails.TryGetValue(name, out var robot))
                {
                    // Retrieve the credentials for the robot
                    if (robotConfig.credentials.TryGetValue(robot.credential, out var credential))
                    {
                        // Return the found robot with its details
                        return new RobotSchema.Robot(
                            name,
                            robot.ip,
                            robot.port,
                            credential.username,
                            credential.password
                        );
                    }
                }
            }
        }

        return null; 
    }
}