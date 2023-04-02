using System.Collections.Generic;
using FP;
using Log;
using Network;

namespace Logic
{
    /// <summary>
    /// entrance for logical client 
    /// </summary>
    public class ClientMain : ISnapshot<ClientSnapshot>
    {
        public ClientNetworkProxy NetworkProxy { get; private set; }
        
        public PlayerInputManager PlayerInputManager { get;private set; }

        /// <summary>
        /// current frame, will increase just before handling next frame data from server
        /// </summary>
        public int CurFrame { get; private set; }

        /// <summary>
        /// deterministic random generator
        /// </summary>
        private FRandom mRand;

        /// <summary>
        /// initial data of the whole gameplay
        /// </summary>
        public GameInitInfo GameInitInfo { get; private set; }

        public UnitManager UnitMgr { get; private set; }

        /// <summary>
        ///  TODO should support multiple player
        /// </summary>
        public Player Player => UnitMgr.Player;

        /// <summary>
        /// result info of the whole gameplay
        /// </summary>
        public ResultInfo Result { get; private set; }

        public GameState State { get; private set; }

        /// <summary>
        /// play mode of current game, affects specific behaviors
        /// </summary>
        private GamePlayMode mPlayMode;

        /// <summary>
        /// whether generate data for presentation
        /// </summary>
        public bool WithRecord => this.mPlayMode == GamePlayMode.Play || this.mPlayMode == GamePlayMode.Replay;

        /// <summary>
        /// whether is replaying,
        /// in replay mode, all frame operations are set before running,
        /// so no need to wait for server's response
        /// </summary>
        public bool IsReplay => this.mPlayMode == GamePlayMode.Replay || this.mPlayMode == GamePlayMode.Validate;

        /// <summary>
        /// time left for next frame processing
        /// </summary>
        private float mFrameCd;

        public void Init(GameInitInfo gameInitInfo, INetwork network, GamePlayMode playMode)
        {
            NetworkProxy = new ClientNetworkProxy(network);
            NetworkProxy.SpFrameHandler += OnSpecialFrame;

            this.mPlayMode = playMode;

            PlayerInputManager = new PlayerInputManager(this);

            InitStatic();

            GameInitInfo = gameInitInfo;


            Result = null;

            mRand = new FRandom(gameInitInfo.RandSeed);
            UnitMgr = new UnitManager(this);
            UnitMgr.Init(gameInitInfo);

            State = GameState.Loading;
        }

        /// <summary>
        /// initialize static fields of all relevant classes
        /// </summary>
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

            //sleep for a specific time interval before processing next frame
            mFrameCd -= deltaTime;
            if (mFrameCd > 0)
            {
                return;
            }

            if (HandleFrame())
            {
                //update one frame
                // if game finished during the process, it's a complicated question whether to abort or finish the simulation
                LogicUpdate();

                //reset sleep timer
                mFrameCd = LogicConst.FrameInterval;
            }
        }

        /// <summary>
        /// fetch and handle frame data from server
        /// </summary>
        /// <returns>the operation succeed or not</returns>
        public bool HandleFrame()
        {
            //jump out if game over
            if (State != GameState.Running)
            {
                return false;
            }

            var frameOperation = NetworkProxy.GetFrameOperation(CurFrame + 1);
            if (frameOperation != null)
            {
                //into new frame
                ++CurFrame;
                //handle operations
                var operations = frameOperation.OperationList;
                if (operations != null)
                {
                    foreach (var operation in operations)
                    {
                        HandleOperation(operation);
                    }
                }
            }
            else
            {
                //in replay mode, empty frame data is ignored
                if (!IsReplay)
                {
                    return false;
                }

                //no frame data for this frame, just start update
                ++CurFrame;
            }

            return true;
        }

        /// <summary>
        /// handles specific operations from server, only for normal frame data
        /// </summary>
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
        /// update whole game world, after handling frame data
        /// </summary>
        public void LogicUpdate()
        {
            UnitMgr.LogicUpdate();
        }

        public void OnGameStart()
        {
            State = GameState.Running;
            //start process when receive first frame(frame 0)
            mFrameCd = 0;
            //wait for frame 0
            CurFrame = -1;
        }

        /// <summary>
        /// handle special frame operation
        /// </summary>
        private void OnSpecialFrame(FrameData frameData)
        {
            if (frameData.OperationList != null)
            {
                var operation = frameData.OperationList[0];
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


        /// <summary>
        /// notify the server that this client is ready to start
        /// </summary>
        public void OnGameReady()
        {
            State = GameState.Ready;
            var operation = new GameReadyOperation();
            NetworkProxy.SendToServer(operation);
        }

        /// <summary>
        /// choose some data from current game state to generate result info,
        /// for server validation
        /// </summary>
        private ResultInfo GenerateResultInfo()
        {
            var posList = new List<FVector2>(UnitMgr.EnemyMap.Count + 1);
            posList.Add(UnitMgr.Player.Pos);
            foreach (var unitPair in UnitMgr.EnemyMap)
            {
                posList.Add(unitPair.Value.Pos);
            }

            return new ResultInfo()
            {
                LastFrame = CurFrame,
                PosList = posList,
            };
        }

        /// <summary>
        /// operation for game finish
        /// </summary>
        public void OnGameFinish()
        {
            if (State == GameState.Finished)
            {
                Logger.Warn($"Duplicate OnGameFinish");
                return;
            }

            State = GameState.Finished;

            Result = GenerateResultInfo();

            //report result to server
            var operation = new GameFinishOperation(Result);
            NetworkProxy.SendToServer(operation);
        }

        #region common methods

        /// <summary>
        /// send operation to server, only for normal operations
        /// </summary>
        public void SendToServer(BaseOperation operation)
        {
            if (State != GameState.Running)
            {
                return;
            }

            Logger.Assert(operation.OpType > OperationType.MinNormal,
                $"${operation.OpType} Not Allowed Here, should greater than OperationType.MinNormal");
            Logger.Info($"[Frame] Client Send {operation}");
            NetworkProxy.SendToServer(operation);
        }

        /// <summary>
        /// generate a random number,
        /// all random number in logic scope should be generated by this method,
        /// should not be called outside logic scope
        /// </summary>
        public int NextRand()
        {
            return mRand.NextRand();
        }

        #endregion

        #region snapshot

        /// <summary>
        /// save all static field values to snapshot,
        /// they don't belong to any instance, so should be saved separately
        /// </summary>
        private static void SaveStatic(StaticSnapshot snapshot)
        {
            snapshot.ClientObject_NextUid = ClientObject.NextUid;
        }

        /// <summary>
        /// revert static field values from snapshot,
        /// they don't belong to any instance, so should be reverted separately
        /// </summary>
        private static void RevertStatic(StaticSnapshot snapshot)
        {
            ClientObject.NextUid = snapshot.ClientObject_NextUid;
        }

        public void SaveToSnapShot(ClientSnapshot snapshot)
        {
            SaveStatic(snapshot.Static);

            snapshot.CurFrame = CurFrame;
            mRand.SaveToSnapShot(snapshot.Rand);
            UnitMgr.SaveToSnapShot(snapshot.UnitMgr);
        }

        public void RevertFromSnapShot(ClientSnapshot snapshot)
        {
            RevertStatic(snapshot.Static);

            CurFrame = snapshot.CurFrame;

            mRand ??= new FRandom();
            mRand.RevertToSnapShot(snapshot.Rand);

            if (UnitMgr == null)
            {
                UnitMgr = new UnitManager(this, snapshot.UnitMgr);
            }
            else
            {
                UnitMgr.RevertFromSnapShot(snapshot.UnitMgr);
            }

            //TODO other fields
        }

        #endregion
    }
}