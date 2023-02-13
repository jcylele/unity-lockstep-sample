using Logic;

namespace Network
{
    /// <summary>
    /// 本地网络
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

        public void SendToClient(FrameOperation frameOperation)
        {
            mClientFunc(frameOperation);
        }

        public void SendToServer(BaseOperation operation)
        {
            mServerFunc(operation);
        }
    }
}