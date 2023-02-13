using System.Collections.Generic;

namespace Logic
{
    /// <summary>
    /// 作战单元管理器
    /// </summary>
    public class UnitManager : ClientObject, ISnapshot<UnitManagerSnapshot>
    {
        public Player Player { get; private set; }

        public SortedDictionary<ulong, Enemy> EnemyMap { get; private set; }

        public UnitManager(ClientMain client) : base(client)
        {
        }

        public UnitManager(ClientMain client, UnitManagerSnapshot snapshot) : base(client, snapshot)
        {
            this.RevertToSnapShot(snapshot, false);
        }

        public void Init(GameInitInfo gameInitInfo)
        {
            EnemyMap = new SortedDictionary<ulong, Enemy>();

            //玩家
            Player = new Player(Client);
            Player.Spawn();

            //敌人
            foreach (var sid in gameInitInfo.EnemySidList)
            {
                var enemy = new Enemy(Client, sid);
                enemy.Spawn();
                EnemyMap.Add(enemy.Uid, enemy);
            }
        }

        public void LogicUpdate()
        {
            Player.LogicUpdate();
            foreach (var unitPair in EnemyMap)
            {
                unitPair.Value.LogicUpdate();
            }

            CheckCollision();
        }

        private void CheckCollision()
        {
            foreach (var unitPair in EnemyMap)
            {
                var deltaPos = unitPair.Value.Pos - Player.Pos;
                var radius = unitPair.Value.Config.radius + Player.Config.radius;
                if (deltaPos.sqLength <= radius * radius)
                {
                    Client.OnGameFinish();
                    return;
                }
            }
        }

        public void SaveToSnapShot(UnitManagerSnapshot snapshot)
        {
            Player.SaveToSnapShot(snapshot.Player);
            foreach (var enemy in EnemyMap)
            {
                var uss = new UnitSnapshot();
                enemy.Value.SaveToSnapShot(uss);
                snapshot.EnemyList.Add(uss);
            }
        }

        public void RevertToSnapShot(UnitManagerSnapshot snapshot, bool needBase)
        {
            if (needBase)
            {
                base.RevertToSnapShot(snapshot, true);
            }

            if (Player == null)
            {
                Player = new Player(Client, snapshot.Player);
            }
            else
            {
                Player.RevertToSnapShot(snapshot.Player, true);
            }

            var curEnemyMap = EnemyMap;
            EnemyMap = new SortedDictionary<ulong, Enemy>();

            foreach (var enemySnapshot in snapshot.EnemyList)
            {
                if (curEnemyMap.TryGetValue(enemySnapshot.Uid, out var enemy))
                {
                    enemy.RevertToSnapShot(enemySnapshot, true);
                }
                else
                {
                    enemy = new Enemy(Client, enemySnapshot);
                    EnemyMap.Add(enemy.Uid, enemy);
                }
            }
        }
    }
}