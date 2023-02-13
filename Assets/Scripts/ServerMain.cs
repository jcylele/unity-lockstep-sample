using System.Collections.Generic;
using Log;
using Logic;
using Network;

namespace Server
{
    /// <summary>
    /// 服务器，只负责转发
    /// </summary>
    public class ServerMain
    {
        private int mCurFrame;
        private float mElapsedTime;
        private bool mRunning;

        private SortedDictionary<int, List<BaseOperation>> mOperationMap;

        public INetwork Network { get; private set; }

        /// <summary>
        /// 初始信息
        /// </summary>
        public InitInfo InitInfo { get; private set; }

        /// <summary>
        /// 结算信息(用于核查验证)
        /// </summary>
        public ResultInfo Result { get; private set; }

        /// <summary>
        /// 战斗记录，用于重播
        /// </summary>
        public GameReport GameReport { get; private set; }

        public void Init(InitInfo initInfo, INetwork network)
        {
            InitInfo = initInfo;
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
            if (mElapsedTime < Const.FrameInterval) return;

            DispatchFrame();
            mElapsedTime -= Const.FrameInterval;
            mCurFrame++;
        }

        /// <summary>
        /// 下发帧到C
        /// </summary>
        private void DispatchFrame()
        {
            mOperationMap.TryGetValue(mCurFrame, out var list);
            var frameOperation = new FrameOperation(mCurFrame, list);
            Network.SendToClient(frameOperation);
        }

        /// <summary>
        /// C上报就绪
        /// </summary>
        private void OnGameReady()
        {
            Logger.Assert(mRunning == false, "Duplicate OnGameReady");

            mRunning = true;
            Result = null;
            GameReport = null;
            mCurFrame = 0;
            mElapsedTime = 0;
            mOperationMap = new SortedDictionary<int, List<BaseOperation>>();

            //通知C开始
            var operation = new GameStartOperation();
            var frameOperation = new FrameOperation(-1, new List<BaseOperation> {operation});
            Network.SendToClient(frameOperation);
        }

        /// <summary>
        /// C上报结算处理，多人结算很久以后再说
        /// </summary>
        private void OnGameFinish(BaseOperation operation)
        {
            mRunning = false;

            var gf = operation as GameFinishOperation;
            Result = gf.Result;

            if (ValidateResult())
            {
                Logger.Info("Game Finished");

                var list = new List<FrameOperation>(mOperationMap.Count);
                foreach (var pair in mOperationMap)
                {
                    list.Add(new FrameOperation(pair.Key, pair.Value));
                }

                GameReport = new GameReport()
                {
                    InitInfo = InitInfo,
                    FrameOperationList = list
                };

                DataBase.Instance.SaveReport(GameReport);
            }
            else
            {
                Logger.Assert(false, "Game Result DisMatch");
            }
        }

        /// <summary>
        /// 接收到C操作
        /// </summary>
        public void OnReceiveOperation(BaseOperation operation)
        {
            Logger.Info($"[Frame] Server Receive {operation} On {mCurFrame}");

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
            else
            {
                if (!mOperationMap.TryGetValue(mCurFrame, out var list))
                {
                    list = new List<BaseOperation>();
                    mOperationMap.Add(mCurFrame, list);
                }

                list.Add(operation);
            }
        }

        /// <summary>
        /// 战斗结果校验
        /// </summary>
        /// <returns>校验是否成功</returns>
        private bool ValidateResult()
        {
            var oldLevel = Logger.Level;
            Logger.SetLevel(LogLevel.Error);

            var client = new ClientMain();
            client.Init(InitInfo, null, false, true);
            client.OnGameReady();
            client.OnGameStart();
            foreach (var frame in mOperationMap)
            {
                client.OnReceiveFrame(new FrameOperation(frame.Key, frame.Value));
            }

            while (client.HandleFrame())
            {
            }

            Logger.SetLevel(oldLevel);

            return Result == client.Result;
        }
    }
}