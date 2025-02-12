using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.Protocols;


namespace Mir_Utilities.RosTools;

public class RosSharpTest
{
      static readonly string uri = "ws://192.168.56.102:9090";

        /*public static void Test()
        {
            RosSocket rosSocket = new RosSocket(new WebSocketNetProtocol(uri));
            

            rosSocket.Close();

            // Subscription:
            string subscription_id = rosSocket.Subscribe<std_msgs.String>("/subscription_test", SubscriptionHandler);
            subscription_id = rosSocket.Subscribe<std_msgs.String>("/subscription_test", SubscriptionHandler);

            // Service Call:
            rosSocket.CallService<rosapi.GetParamRequest, rosapi.GetParamResponse>("/rosapi/get_param", ServiceCallHandler, new rosapi.GetParamRequest("/rosdistro", "default"));

            // Service Response:
            string service_id = rosSocket.AdvertiseService<std_srvs.TriggerRequest, std_srvs.TriggerResponse>("/service_response_test", ServiceResponseHandler);

            Console.WriteLine("Press any key to unsubscribe...");
            Console.ReadKey(true);
            rosSocket.Unadvertise(publication_id);
            rosSocket.Unsubscribe(subscription_id);
            rosSocket.UnadvertiseService(service_id);

            Console.WriteLine("Press any key to close...");
            Console.ReadKey(true);

        }
        private static void SubscriptionHandler(std_msgs.String message)
        {
            Console.WriteLine((message).data);
        }

        private static void ServiceCallHandler(rosapi.GetParamResponse message)
        {
            Console.WriteLine("ROS distro: " + message.value);
        }

        private static bool ServiceResponseHandler(std_srvs.TriggerRequest arguments, out std_srvs.TriggerResponse result)
        {
            result = new std_srvs.TriggerResponse(true, "service response message");
            return true;
        }*/
}