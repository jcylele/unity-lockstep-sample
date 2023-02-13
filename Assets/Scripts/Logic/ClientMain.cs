using System.Collections.Generic;
using FixMath;
using Log;
using Network;

namespace Logic
{
    /// <summary>
    /// 客户端逻辑入口
    /// </summary>
    public class ClientMain : ISnapshot<ClientSnapshot>
    {
        /// <summary>
        /// 当前帧
        /// </summary>
        public int CurFrame { get; private set; }

        /// <summary>
        /// 等待处理的帧数据，按帧排序过
        /// </summary>
        private LinkedList<FrameOperation> mWaitFrames;

        /// <summary>
        /// 确定随机数
        /// </summary>
        private FRandom mRand;

        /// <summary>
        /// 网络接口
        /// </summary>
        private INetwork mNetwork;

        public GameInitInfo GameInitInfo { get; private set; }
        public UnitManager UnitMgr { get; private set; }
        public Player Player => UnitMgr.Player;
        public ResultInfo Result { get; private set; }
        public GameState State { get; private set; }

        /// <summary>
        /// 是否开启数据缓存，供表现层使用
        /// </summary>
        public bool WithRecord { get; private set; }

        public bool IsReplay { get; private set; }

        private float mFrameCd;

        public void Init(GameInitInfo gameInitInfo, INetwork network, bool withRecord, bool isReplay = false)
        {
            InitStatic();

            GameInitInfo = gameInitInfo;
            mNetwork = network;
            WithRecord = withRecord;
            IsReplay = isReplay;

            Result = null;
            mWaitFrames = new LinkedList<FrameOperation>();

            mRand = new FRandom(gameInitInfo.RandSeed);
            UnitMgr = new UnitManager(this);
            UnitMgr.Init(gameInitInfo);

            State = GameState.Loading;
        }

        private static void InitStatic()
        {
            ClientObject.NextUid = 0;
        }

        public void Update(float deltaTime)
        {
            if (State != GameState.Running)
            {
                return;
            }

            mFrameCd -= deltaTime;
            if (mFrameCd > 0)
            {
                return;
            }

            if (HandleFrame())
            {
                mFrameCd = Const.FrameInterval;
            }
        }

        public bool HandleFrame()
        {
            if (State != GameState.Running)
            {
                return false;
            }

            var first = mWaitFrames.First;
            //没有下一帧数据
            if (first == null || first.Value.FrameIndex != CurFrame + 1)
            {
                if (!IsReplay)
                {
                    return false;
                }

                //新帧开始
                ++CurFrame;
            }
            else
            {
                //移除该帧
                mWaitFrames.RemoveFirst();
                //新帧开始
                ++CurFrame;
                //处理操作下发
                var operations = first.Value.OperationList;
                if (operations != null)
                {
                    foreach (var operation in operations)
                    {
                        HandleOperation(operation);
                    }
                }
            }

            //推进一帧
            //此时可能会结算，后续的Update执行会不会有问题，待议
            LogicUpdate();

            return true;
        }

        private void HandleOperation(BaseOperation operation)
        {
            switch (operation.OpType)
            {
                case OperationType.Key:
                    var keyOperation = operation as KeyOperation;
                    UnitMgr.Player.OnKey(keyOperation.Key, keyOperation.KeyOpType);
                    break;
                default:
                    Logger.Assert(false, $"Invalid OperationType: {operation.OpType}");
                    break;
            }
        }

        /// <summary>
        /// 推进一帧
        /// </summary>
        private void LogicUpdate()
        {
            UnitMgr.LogicUpdate();
        }

        public void OnGameStart()
        {
            State = GameState.Running;
            mFrameCd = 0;
            CurFrame = -1;
        }

        /// <summary>
        /// 接收到服务器下发帧操作
        /// </summary>
        public void OnReceiveFrame(FrameOperation frameOperation)
        {
            switch (frameOperation.FrameIndex)
            {
                case -1: //特殊操作
                {
                    Logger.Info($"[Frame] Client Receive {frameOperation}");

                    if (frameOperation.OperationList != null)
                    {
                        var operation = frameOperation.OperationList[0];
                        if (operation.OpType == OperationType.GameStart)
                        {
                            OnGameStart();
                        }
                    }
                    else
                    {
                        Logger.Error($"WTF");
                    }
                }
                    break;
                case -2:
                {
                    Logger.Error("WTF");
                }
                    break;
                default:
                {
                    if (frameOperation.OperationList != null)
                    {
                        Logger.Info($"[Frame] Client Receive {frameOperation}");
                    }

                    var tmp = mWaitFrames.Last;
                    while (tmp != null)
                    {
                        if (frameOperation.FrameIndex > tmp.Value.FrameIndex)
                        {
                            mWaitFrames.AddAfter(tmp, frameOperation);
                            return;
                        }

                        tmp = tmp.Previous;
                    }

                    mWaitFrames.AddFirst(frameOperation);
                }
                    break;
            }
        }

        /// <summary>
        /// 准备就绪(资源加载等操作完成)
        /// </summary>
        public void OnGameReady()
        {
            State = GameState.Ready;
            var operation = new GameReadyOperation();
            mNetwork?.SendToServer(operation);
        }

        public void OnGameFinish()
        {
            if (State == GameState.Finished)
            {
                Logger.Warn($"Duplicate OnGameFinish");
                return;
            }

            State = GameState.Finished;

            var posList = new List<FVector2>(UnitMgr.EnemyMap.Count + 1);
            posList.Add(UnitMgr.Player.Pos);
            foreach (var unitPair in UnitMgr.EnemyMap)
            {
                posList.Add(unitPair.Value.Pos);
            }

            Result = new ResultInfo()
            {
                LastFrame = CurFrame,
                PosList = posList,
            };

            //上报结算
            var operation = new GameFinishOperation(Result);
            mNetwork?.SendToServer(operation);
        }

        #region 公共方法

        /// <summary>
        /// 发送单操作到S,只针对普通操作
        /// </summary>
        /// <param name="operation">单操作</param>
        public void SendToServer(BaseOperation operation)
        {
            if (State != GameState.Running)
            {
                return;
            }

            Logger.Assert(operation.OpType > OperationType.MinNormal,
                $"${operation.OpType} Not Allowed Here, should greater than OperationType.MinNormal");
            Logger.Info($"[Frame] Client Send {operation}");
            mNetwork?.SendToServer(operation);
        }

        /// <summary>
        /// 随机数生成器，Logic必须使用这个
        /// </summary>
        /// <returns>随机数</returns>
        public int NextRand()
        {
            return mRand.NextRand();
        }

        #endregion

        private static void SaveStatic(StaticSnapshot snapshot)
        {
            snapshot.ClientObject_NextUid = ClientObject.NextUid;
        }

        public void SaveToSnapShot(ClientSnapshot snapshot)
        {
            SaveStatic(snapshot.Static);

            snapshot.CurFrame = CurFrame;
            mRand.SaveToSnapShot(snapshot.Rand);
            UnitMgr.SaveToSnapShot(snapshot.UnitMgr);
        }

        private static void RevertStatic(StaticSnapshot snapshot)
        {
            ClientObject.NextUid = snapshot.ClientObject_NextUid;
        }

        public void RevertToSnapShot(ClientSnapshot snapshot, bool needBase)
        {
            RevertStatic(snapshot.Static);

            CurFrame = snapshot.CurFrame;

            if (mRand == null)
            {
                mRand = new FRandom(snapshot.Rand);
            }
            else
            {
                mRand.RevertToSnapShot(snapshot.Rand, true);
            }

            if (UnitMgr == null)
            {
                UnitMgr = new UnitManager(this, snapshot.UnitMgr);
            }
            else
            {
                UnitMgr.RevertToSnapShot(snapshot.UnitMgr, true);
            }

            //TODO 设置mWaitFrames，Result等
        }
    }
}