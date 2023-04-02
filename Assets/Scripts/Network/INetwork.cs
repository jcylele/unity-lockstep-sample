using Logic;

namespace Network
{
    /// <summary>
    /// client receives a frame data containing multiple operations at one time from server
    /// </summary>
    public delegate void ClientRecvFunc(FrameData frameData);

    /// <summary>
    /// server receives an operation at one time from client
    /// </summary>
    public delegate void ServerRecvFunc(BaseOperation operation);

    /// <summary>
    /// client and server send msg to each other by network
    /// </summary>
    public interface INetwork
    {
        /// <summary>
        /// init the msg receive functions of client and server
        /// </summary>
        /// <param name="clientFunc"> receive function of client </param>
        /// <param name="serverFunc"> receive function of server </param>
        void Init(ClientRecvFunc clientFunc, ServerRecvFunc serverFunc);

        void Update(float deltaTime);

        /// <summary>
        /// server try to send msg to client
        /// </summary>
        void SendToClient(FrameData frameData);

        /// <summary>
        /// client try to send msg to server
        /// </summary>
        void SendToServer(BaseOperation operation);
    }
}