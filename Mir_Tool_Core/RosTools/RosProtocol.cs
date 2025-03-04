using System.Net.WebSockets;
using System.Threading.Channels;
using RosSharp.RosBridgeClient.Protocols;

namespace Mir_Utilities.RosTools;

public class RosProtocol
{
 
    public class ModifiedWebSocketNetProtocol : IProtocol
    {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private ClientWebSocket clientWebSocket;
        private readonly Uri uri;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly CancellationToken cancellationToken;
        private ManualResetEvent IsConnected = new ManualResetEvent(false);
        private AutoResetEvent IsReadyToSend = new AutoResetEvent(true);

        private const int ReceiveChunkSize = 1024;
        private const int SendChunkSize = 1024;

        private ChannelReader<ArraySegment<byte>> reader;
        private ChannelWriter<ArraySegment<byte>> writer;

        private Task listener;
        private Task sender;

        public event EventHandler OnReceive;
        public event EventHandler OnConnected;
        public event EventHandler OnClosed;

        public ModifiedWebSocketNetProtocol(string uriString, string authID, int queueSize = 1000)
        {
            Channel<ArraySegment<byte>> channel = Channel.CreateUnbounded<ArraySegment<byte>>(new UnboundedChannelOptions()
            {
                AllowSynchronousContinuations = false,
                SingleReader = true,
                SingleWriter = false,
            });

            reader = channel.Reader;
            writer = channel.Writer;

            clientWebSocket = new ClientWebSocket();
            clientWebSocket.Options.SetRequestHeader("Authorization", authID );
            clientWebSocket.Options.CollectHttpResponseDetails = true;
            
            uri = new Uri(uriString);
            cancellationToken = cancellationTokenSource.Token;
        }

        public void Connect()
        {
            Task.Run(() => ConnectAsync());
        }

        public async void ConnectAsync()
        {
            try
            {
                await clientWebSocket.ConnectAsync(uri, cancellationToken);


            }catch(Exception e)
            {
                LOGGER.Info(clientWebSocket.CloseStatusDescription); 
                LOGGER.Error(e.Message);
                LOGGER.Error(e.StackTrace);
            }

            IsConnected.Set();
            OnConnected?.Invoke(null, EventArgs.Empty);
            listener = Task.Run(StartListen);
            sender = Task.Run(StartSend);
        }

        public void Close()
        {
            if (IsAlive())
            {
                writer.Complete();
            }
        }

        public bool IsAlive()
        {
            return clientWebSocket.State == WebSocketState.Open;
        }

        public void Send(byte[] message)
        {
            Send(new ArraySegment<byte>(message));
        }

        public void Send(ArraySegment<byte> msg)
        {
            if (!writer.TryWrite(msg))
            {
                throw new Exception();
            }
        }

        private async Task StartSend()
        {
            while (await reader.WaitToReadAsync())
            {
                if (reader.TryRead(out ArraySegment<byte> message))
                {
                    if (clientWebSocket.State != WebSocketState.Open)
                        throw new WebSocketException(WebSocketError.InvalidState, "Error Sending Message. WebSocket State is: " + clientWebSocket.State);

                    int messageCount = (int)Math.Ceiling((double)message.Count / SendChunkSize);

                    for (int i = 0; i < messageCount; i++)
                    {
                        int offset = SendChunkSize * i;
                        bool endOfMessage = (i == messageCount - 1);
                        int count = endOfMessage ? message.Count - offset : SendChunkSize;
                        await clientWebSocket.SendAsync(new ArraySegment<byte>(message.Array, offset, count), WebSocketMessageType.Binary, endOfMessage, cancellationToken).ConfigureAwait(false);
                    }
                }
            }
            // close the socket (listener will therminate after that)
            clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None).Wait();
            IsConnected.Reset();
            OnClosed?.Invoke(null, EventArgs.Empty);
        }

        private async Task StartListen()
        {
            byte[] buffer = new byte[ReceiveChunkSize];

            while (clientWebSocket.State == WebSocketState.Open)
            {
                MemoryStream memoryStream = new MemoryStream();
                WebSocketReceiveResult result;
                do
                {
                    result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);

                    if (result.MessageType == WebSocketMessageType.Close)
                        return;

                    memoryStream.Write(buffer, 0, result.Count);

                } while (!result.EndOfMessage);

                OnReceive?.Invoke(this, new MessageEventArgs(memoryStream.ToArray()));
            }
        }
    }

}