using System.Collections.Generic;

namespace Logic
{
    /// <summary>
    /// manager of all units
    /// </summary>
    public class UnitManager : ClientObject, ISnapshot<UnitManagerSnapshot>
    {
        public Player Player { get; private set; }

        /// <summary>
        /// all enemies, sorted by uid, for deterministic purpose
        /// </summary>
        public SortedDictionary<ulong, Enemy> EnemyMap { get; private set; }

        public UnitManager(ClientMain client) : base(client)
        {
        }

        public UnitManager(ClientMain client, UnitManagerSnapshot snapshot) : base(client, snapshot)
        {
            this.RevertFromSnapShot(snapshot);
        }

        public void Init(GameInitInfo gameInitInfo)
        {
            EnemyMap = new SortedDictionary<ulong, Enemy>();

            //player
            Player = new Player(Client);
            Player.Spawn();

            //enemies
            foreach (var sid in gameInitInfo.EnemySidList)
            {
                var enemy = new Enemy(Client, sid);
                enemy.Spawn();
                EnemyMap.Add(enemy.Uid, enemy);
            }
        }

        protected override void InnerUpdate()
        {
            Player.LogicUpdate();
            foreach (var unitPair in EnemyMap)
            {
                unitPair.Value.LogicUpdate();
            }

            CheckCollision();
        }

        /// <summary>
        /// check collision between player and enemies,
        /// if collision happens, game over
        /// </summary>
        private void CheckCollision()
        {
            foreach (var unitPair in EnemyMap)
            {
                var deltaPos = unitPair.Value.Pos - Player.Pos;
                var radius = unitPair.Value.Config.radius + Player.Config.radius;
                if (deltaPos.SqLength <= radius * radius)
                {
                    Client.OnGameFinish();
                    return;
                }
            }
        }

        #region Snapshot

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

        public void RevertFromSnapShot(UnitManagerSnapshot snapshot)
        {
            base.RevertFromSnapShot(snapshot);

            if (Player == null)
            {
                Player = new Player(Client, snapshot.Player);
            }
            else
            {
                Player.RevertFromSnapShot(snapshot.Player);
            }

            var curEnemyMap = EnemyMap;
            EnemyMap = new SortedDictionary<ulong, Enemy>();

            foreach (var enemySnapshot in snapshot.EnemyList)
            {
                if (curEnemyMap.TryGetValue(enemySnapshot.Uid, out var enemy))
                {
                    enemy.RevertFromSnapShot(enemySnapshot);
                }
                else
                {
                    enemy = new Enemy(Client, enemySnapshot);
                    EnemyMap.Add(enemy.Uid, enemy);
                }
            }
        }

        #endregion
    }
}