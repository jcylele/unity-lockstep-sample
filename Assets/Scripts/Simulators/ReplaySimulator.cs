using Log;
using Logic;

namespace Simulators
{
    /// <summary>
    /// replay a game according to initial info and frame operations
    /// </summary>
    public class ReplaySimulator : ISimulator
    {
        public ClientMain Client { get; }

        public ReplaySimulator(GameReport report)
        {
            Logger.SetLevel(LogLevel.Info);
            Logger.SetLogger(new MonoLogger());

            Client = new ClientMain();
            Client.Init(report.GameInitInfo, null, GamePlayMode.Replay);
            if (report.FrameOperationList != null)
            {
                foreach (var frameOperation in report.FrameOperationList)
                {
                    Client.NetworkProxy.OnReceiveFrameData(frameOperation);
                }
            }
        }

        public void Update(float deltaTime)
        {
            Client.Update(deltaTime);
        }
    }
}