using Mir_Utilities.Common;
using Mir_Utilities.RosTools.Schema;
using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.Protocols;
using WebSocketSharp;
using std_msgs = RosSharp.RosBridgeClient.MessageTypes.Std;

namespace Mir_Utilities.RosTools;

public class RosGetDiagnosisInfomation
{
    private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    public static RobotHealth GetRobotHealth(RobotSchema.Robot robot)
    {
        string websocketURI = $"ws://{robot.Ip}/rosbridge/";
        RosSocket customRosSocket = new Ross
        RosSocket rosSocket = new RosSocket(new RosProtocol.ModifiedWebSocketNetProtocol(websocketURI, robot.AuthId), RosSocket.SerializerEnum.Newtonsoft_JSON);
        /*RosSocket rosSocket = new RosSocket(new WebSocketNetProtocol(websocketURI));*/
        RobotHealth robotHealth = new RobotHealth();
        int infoCount = 0;
        string subscriptionId = rosSocket.Subscribe<FakeRosMessage>("/diagnostics_agg", msg =>
        {
            infoCount++;
            LOGGER.Info("Received DiagnosticArray message");
            // Print out the contents of the received DiagnosticArray message
            
            LOGGER.Info(msg.ToString());
            /*LOGGER.Info($"Header seq:{msg.dyn.header.seq} , frame_id: {msg.dyn.header.frame_id}");
            
            foreach (var status in msg.dyn.status)
            {
                LOGGER.Info($"Status: {status.name}, Message: {status.message}");
                foreach (var kv in status.values)
                {
                    LOGGER.Info($"Key: {kv.key}, Value: {kv.value}");
                }
            }*/
        });
        LOGGER.Info($"subscriber id is {subscriptionId}");
        while (infoCount < 5)
        {
            
        }
        LOGGER.Info($"subscriber {subscriptionId} is deleted ");
        rosSocket.Close();


        return robotHealth;
    }

}