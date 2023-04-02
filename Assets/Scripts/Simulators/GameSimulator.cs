using Log;
using Logic;
using Network;

namespace Simulators
{
    /// <summary>
    /// the normal game simulator, contains client and server
    /// <para>with presentation, waits for packet from server</para>
    /// </summary>
    public class GameSimulator : ISimulator
    {
        private INetwork Network { get; }
        public ClientMain Client { get; }
        private ServerMain Server { get; }

        public GameSimulator(GameInitInfo gameInitInfo, INetwork network)
        {
            Logger.SetLevel(LogLevel.Debug);
            Logger.SetLogger(new MonoLogger());

            // var network = new NetworkSimulator();
            network.Init(this.OnClientReceive, this.OnServerReceive);
            Network = network;

            Client = new ClientMain();
            Client.Init(gameInitInfo, network, GamePlayMode.Play);

            Server = new ServerMain();
            Server.Init(gameInitInfo, network);
        }

        public void Update(float deltaTime)
        {
            Network.Update(deltaTime);
            Server.Update(deltaTime);
            Client.Update(deltaTime);
        }

        void OnClientReceive(FrameData frameData)
        {
            Client.NetworkProxy.OnReceiveFrameData(frameData);
        }

        void OnServerReceive(BaseOperation operation)
        {
            Server.OnReceiveOperation(operation);
        }
    }
}