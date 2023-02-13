// using System.Collections.Generic;
// using Logic;
// using UnityEngine;
//
// namespace Mono
// {
//     /// <summary>
//     /// 客户端表现入口
//     /// </summary>
//     public class ShowMain
//     {
//         private Dictionary<int, GameObject> prefabMap;
//         private Dictionary<int, ResourceRequest> loadMap;
//         public Transform UnitRoot { get; set; }
//         private ClientMain Client;
//         private bool mLoading = false;
//
//         public void Init(InitInfo initInfo, ClientMain client)
//         {
//             Client = client;
//             prefabMap = new Dictionary<int, GameObject>();
//             loadMap = new Dictionary<int, ResourceRequest>();
//
//             Load(0);
//             foreach (var sid in initInfo.EnemySidList)
//             {
//                 Load(sid);
//             }
//
//             mLoading = true;
//         }
//
//         private void Load(int sid)
//         {
//             if (prefabMap.ContainsKey(sid))
//             {
//                 return;
//             }
//
//             if (loadMap.ContainsKey(sid))
//             {
//                 return;
//             }
//
//             var config = UnitConfig.GetConfig(sid);
//             var rr = Resources.LoadAsync<GameObject>(config.prefabPath);
//             loadMap.Add(sid, rr);
//         }
//
//         public void Update(float deltaTime)
//         {
//             if (mLoading)
//             {
//                 foreach (var load in loadMap)
//                 {
//                     if (load.Value.isDone)
//                     {
//                         prefabMap.Add(load.Key, load.Value.asset as GameObject);
//                     }
//                 }
//
//                 foreach (var o in prefabMap)
//                 {
//                     loadMap.Remove(o.Key);
//                 }
//
//                 if (loadMap.Count != 0) return;
//
//                 mLoading = false;
//                 SpawnUnits();
//                 Client.OnGameReady();
//             }
//         }
//
//         private void SpawnUnits()
//         {
//             var playerGo = Object.Instantiate(prefabMap[0], UnitRoot);
//             playerGo.name = "player";
//             var playerBehaviour = playerGo.GetComponent<PlayerBehaviour>();
//             playerBehaviour.Player = Client.Player;
//
//             foreach (var enemyPair in Client.UnitMgr.EnemyMap)
//             {
//                 var enemyGo = Object.Instantiate(prefabMap[enemyPair.Value.Sid], UnitRoot);
//                 enemyGo.name = $"enemy {enemyPair.Key}";
//                 var enemyBehaviour = enemyGo.GetComponent<EnemyBehaviour>();
//                 enemyBehaviour.Enemy = enemyPair.Value;
//             }
//         }
//     }
// }