using Logic;

namespace Network
{
    public delegate void ClientRecvFunc(FrameOperation frameOperation);

    public delegate void ServerRecvFunc(BaseOperation operation);

    public interface INetwork
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="clientFunc"></param>
        /// <param name="serverFunc"></param>
        void Init(ClientRecvFunc clientFunc, ServerRecvFunc serverFunc);

        /// <summary>
        /// 更新
        /// </summary>
        void Update(float deltaTime);

        /// <summary>
        /// S向C发送帧操作
        /// </summary>
        void SendToClient(FrameOperation frameOperation);

        /// <summary>
        /// C向S发送单操作
        /// </summary>
        void SendToServer(BaseOperation operation);
    }
}