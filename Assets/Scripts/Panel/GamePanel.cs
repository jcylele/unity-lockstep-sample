using System.Collections.Generic;
using Logic;
using Mono;
using Network;
using UnityEngine;

namespace Panel
{
    public class GamePanel : PanelSingleton<GamePanel>
    {
        private readonly Dictionary<int, GameObject> mPrefabMap = new Dictionary<int, GameObject>();
        private readonly Dictionary<int, ResourceRequest> mLoadMap = new Dictionary<int, ResourceRequest>();
        private Transform mUnitRoot;
        private ISimulator mSimulator;

        new void Awake()
        {
            base.Awake();
            this.gameObject.SetActive(false);
        }

        void OnDisable()
        {
            if (mUnitRoot != null)
            {
                Destroy(mUnitRoot.gameObject);
                mUnitRoot = null;
            }
        }

        private void LoadAll(InitInfo initInfo)
        {
            Load(0);
            foreach (var sid in initInfo.EnemySidList)
            {
                Load(sid);
            }
        }

        private void Load(int sid)
        {
            if (mPrefabMap.ContainsKey(sid))
            {
                return;
            }

            if (mLoadMap.ContainsKey(sid))
            {
                return;
            }

            var config = UnitConfig.GetConfig(sid);
            var rr = Resources.LoadAsync<GameObject>(config.prefabPath);
            mLoadMap.Add(sid, rr);
        }

        void UpdateLoading()
        {
            foreach (var load in mLoadMap)
            {
                if (load.Value.isDone)
                {
                    mPrefabMap.Add(load.Key, load.Value.asset as GameObject);
                }
            }

            foreach (var o in mPrefabMap)
            {
                mLoadMap.Remove(o.Key);
            }

            if (mLoadMap.Count != 0) return;

            SpawnUnits();
            mSimulator.Client.OnGameReady();
        }

        void Update()
        {
            mSimulator.Update(Time.deltaTime);

            switch (mSimulator.Client.State)
            {
                case GameState.Loading:
                    UpdateLoading();
                    break;
                case GameState.Ready:
                {
                    if (mSimulator.Client.IsReplay)
                    {
                        mSimulator.Client.OnGameStart();
                    }

                    break;
                }
            }
        }

        private void SpawnUnits()
        {
            if (mUnitRoot != null)
            {
                Destroy(mUnitRoot.gameObject);
                mUnitRoot = null;
            }

            var go = new GameObject("UnitRoot");
            mUnitRoot = go.transform;
            mUnitRoot.SetParent(this.transform);
            mUnitRoot.localPosition = Vector3.zero;

            var client = mSimulator.Client;
            var playerGo = Instantiate(mPrefabMap[0], mUnitRoot);
            playerGo.name = "player";
            var playerBehaviour = playerGo.GetComponent<PlayerBehaviour>();
            playerBehaviour.Player = client.Player;

            foreach (var enemyPair in client.UnitMgr.EnemyMap)
            {
                var enemyGo = Instantiate(mPrefabMap[enemyPair.Value.Sid], mUnitRoot);
                enemyGo.name = $"enemy {enemyPair.Key}";
                var enemyBehaviour = enemyGo.GetComponent<EnemyBehaviour>();
                enemyBehaviour.Enemy = enemyPair.Value;
            }
        }

        public void StartGame(InitInfo initInfo)
        {
            var network = new NetworkSimulator();
            network.SetLatency(0.05f, 0.45f);
            mSimulator = new GameSimulator(initInfo, network);

            LoadAll(initInfo);

            this.ShowGame(true);
        }

        public void StartReplay(GameReport report)
        {
            mSimulator = new ReplaySimulator(report);

            LoadAll(report.InitInfo);
            
            this.ShowGame(true);
        }

        public void ExitGame()
        {
            mLoadMap.Clear();

            mSimulator = null;

            this.ShowGame(false);
        }

        public void SaveSnapshot()
        {
            var snapshot = new ClientSnapshot();
            mSimulator.Client.SaveToSnapShot(snapshot);
            // DataBase.Instance.TestSnapshot(snapshot);
        }
    }
}