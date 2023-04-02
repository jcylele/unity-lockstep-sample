using System.Collections.Generic;
using Log;
using Logic;
using Network;

namespace Simulators
{
    /// <summary>
    /// a simple server, for transmitting data between clients and validating the result
    /// </summary>
    public class ServerMain
    {
        private int mCurFrame;
        private float mElapsedTime;
        private bool mRunning;

        private SortedDictionary<int, List<BaseOperation>> mOperationMap;

        public INetwork Network { get; private set; }

        /// <summary>
        /// initial game info
        /// </summary>
        public GameInitInfo GameInitInfo { get; private set; }

        public void Init(GameInitInfo gameInitInfo, INetwork network)
        {
            GameInitInfo = gameInitInfo;
            Network = network;
            mRunning = false;
        }

        public void Update(float deltaTime)
        {
            if (!mRunning)
            {
                return;
            }

            mElapsedTime += deltaTime;
            if (mElapsedTime < LogicConst.FrameInterval) return;

            DispatchFrame();
            mElapsedTime -= LogicConst.FrameInterval;
            mCurFrame++;
        }

        /// <summary>
        /// dispatch operations in this frame to clients
        /// </summary>
        private void DispatchFrame()
        {
            mOperationMap.TryGetValue(mCurFrame, out var list);
            var frameOperation = new FrameData(mCurFrame, list);
            Network.SendToClient(frameOperation);
        }

        /// <summary>
        /// client notifies server that it's ready to start game
        /// </summary>
        private void OnGameReady()
        {
            Logger.Assert(mRunning == false, "Duplicate OnGameReady");

            mRunning = true;
            mCurFrame = 0;
            mElapsedTime = 0;
            mOperationMap = new SortedDictionary<int, List<BaseOperation>>();

            //notify clients that game is started
            var operation = new GameStartOperation();
            var frameOperation = new FrameData(-1, new List<BaseOperation> { operation });
            Network.SendToClient(frameOperation);
        }

        /// <summary>
        /// client notifies server that game is finished
        /// </summary>
        private void OnGameFinish(BaseOperation operation)
        {
            mRunning = false;

            var gf = operation as GameFinishOperation;
            var clientResult = gf.Result;
            var serverResult = RerunGame();

            if (clientResult == serverResult)
            {
                Logger.Info("Game Finished");

                //save report
                var list = new List<FrameData>(mOperationMap.Count);
                foreach (var pair in mOperationMap)
                {
                    list.Add(new FrameData(pair.Key, pair.Value));
                }

                var gameReport = new GameReport()
                {
                    GameInitInfo = GameInitInfo,
                    FrameOperationList = list
                };

                GameReportCenter.Instance.SaveReport(gameReport);
            }
            else
            {
                Logger.Assert(false, "Game Result DisMatch");
            }
        }

        /// <summary>
        /// receive operation from client
        /// </summary>
        public void OnReceiveOperation(BaseOperation operation)
        {
            Logger.Info($"[Frame] Server Receive {operation} On {mCurFrame}");

            // special operations
            if (operation.OpType < OperationType.MinNormal)
            {
                switch (operation.OpType)
                {
                    case OperationType.GameReady:
                        OnGameReady();
                        break;
                    case OperationType.GameFinish:
                        OnGameFinish(operation);
                        break;
                    default:
                        Logger.Assert(false, $"Invalid Operation Type: {operation.OpType}");
                        break;
                }
            }
            else // normal operations
            {
                // add operation to data of this frame
                if (!mOperationMap.TryGetValue(mCurFrame, out var list))
                {
                    list = new List<BaseOperation>();
                    mOperationMap.Add(mCurFrame, list);
                }

                list.Add(operation);
            }
        }

        /// <summary>
        /// replay the game to generate a new result
        /// </summary>
        /// <returns>game result</returns>
        private ResultInfo RerunGame()
        {
            var oldLevel = Logger.Level;
            Logger.SetLevel(LogLevel.Error);

            var client = new ClientMain();
            client.Init(GameInitInfo, null, GamePlayMode.Validate);
            client.OnGameReady();
            client.OnGameStart();
            foreach (var frame in mOperationMap)
            {
                client.NetworkProxy.OnReceiveFrameData(new FrameData(frame.Key, frame.Value));
            }

            while (client.HandleFrame())
            {
                client.LogicUpdate();
            }

            Logger.SetLevel(oldLevel);

            return client.Result;
        }
    }
}