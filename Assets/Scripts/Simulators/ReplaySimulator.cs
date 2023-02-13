using Log;
using Logic;

namespace Mono
{
    public class ReplaySimulator : ISimulator
    {
        public ClientMain Client { get; }

        public ReplaySimulator(GameReport report)
        {
            Logger.SetLevel(LogLevel.Info);
            Logger.SetLogger(new MonoLogger());

            Client = new ClientMain();
            Client.Init(report.InitInfo, null, true, true);
            if (report.FrameOperationList != null)
            {
                foreach (var frameOperation in report.FrameOperationList)
                {
                    Client.OnReceiveFrame(frameOperation);
                }
            }
        }

        public void Update(float deltaTime)
        {
            Client.Update(deltaTime);
        }
    }
}