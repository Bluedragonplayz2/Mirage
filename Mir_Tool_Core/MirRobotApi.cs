namespace Mir_Utilities;

public class MirRobotApi
{
    public struct MiRRobot(String ip, String authId) //Includes the IP and Generated authId for the robot
    {
        public readonly String Ip { get; } = ip;
        public readonly String AuthId { get; } = authId;

    }
    
    private static ApiCaller _api;
    
    //initialize the API caller with the robot's IP and authId 
    public MirRobotApi(MiRRobot robot)
    {
         _api = new ApiCaller(robot.Ip, robot.AuthId);

    }
    
    //future addition to use tokens to avoid tossing around authIds

    
    public Boolean VerifyConnection()
    {
        try
        {
            dynamic test = _api.GetApi("status").Result;
            return true;
        }
        catch (Exception)
        {
            
            return false;
        }
    }
    public async Task<Boolean> VerifyConnectionAsync()
    {
        try
        {
            await _api.GetApi("status");
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    
   







    
    
    
 




        

   


}