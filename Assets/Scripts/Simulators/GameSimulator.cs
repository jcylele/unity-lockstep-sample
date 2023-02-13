using Log;
using Logic;
using Network;
using Server;

namespace Mono
{
    /// <summary>
    /// 游戏沙盒模拟器
    /// </summary>
    public class GameSimulator : ISimulator
    {
        public INetwork Network { get; }
        public ClientMain Client { get; }
        public ServerMain Server { get; }

        public GameSimulator(GameInitInfo gameInitInfo, INetwork network)
        {
            Logger.SetLevel(LogLevel.Debug);
            Logger.SetLogger(new MonoLogger());

            // var network = new NetworkSimulator();
            network.Init(this.OnClientReceive, this.OnServerReceive);
            Network = network;

            Client = new ClientMain();
            Client.Init(gameInitInfo, network, true);

            Server = new ServerMain();
            Server.Init(gameInitInfo, network);
        }

        public void Update(float deltaTime)
        {
            Network.Update(deltaTime);
            Server.Update(deltaTime);
            Client.Update(deltaTime);
        }

        void OnClientReceive(FrameOperation frameOperation)
        {
            Client.OnReceiveFrame(frameOperation);
        }

        void OnServerReceive(BaseOperation operation)
        {
            Server.OnReceiveOperation(operation);
        }
    }
}