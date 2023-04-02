using Logic;

namespace Network
{
    /// <summary>
    /// simulate local server, no lag and no packet loss
    /// </summary>
    public class NetworkLocal : INetwork
    {
        private ClientRecvFunc mClientFunc;
        private ServerRecvFunc mServerFunc;

        public void Init(ClientRecvFunc clientFunc, ServerRecvFunc serverFunc)
        {
            mClientFunc = clientFunc;
            mServerFunc = serverFunc;
        }

        public void Update(float deltaTime)
        {
        }

        public void SendToClient(FrameData frameData)
        {
            mClientFunc(frameData);
        }

        public void SendToServer(BaseOperation operation)
        {
            mServerFunc(operation);
        }
    }
}