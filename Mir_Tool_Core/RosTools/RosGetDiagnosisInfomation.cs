using Mir_Utilities.Common;
using Mir_Utilities.RosTools.Schema;
using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.Protocols;
using std_msgs = RosSharp.RosBridgeClient.MessageTypes.Std;

namespace Mir_Utilities.RosTools;

public class RosGetDiagnosisInfomation
{
    private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    public static RobotHealth GetRobotHealth(RobotSchema.Robot robot)
    {
        string websocketURI = $"ws://{robot.Ip}:{robot.Port}/rosbridge/";
        
        RosSocket rosSocket = new RosSocket(new WebSocketNetProtocol(websocketURI));
        RobotHealth robotHealth = new RobotHealth();
        string subscriptionId = rosSocket.Subscribe<std_msgs.String>("/diagnostics_agg", message =>
        {
            LOGGER.Info(message);
        });

        rosSocket.Close();


        return robotHealth;
    }

}