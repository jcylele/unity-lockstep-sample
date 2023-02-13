using FixMath;

namespace Logic
{
    /// <summary>
    /// 逻辑敌人
    /// </summary>
    public class Enemy : Unit
    {
        public Enemy(ClientMain client, int sid) : base(client, sid)
        {
        }

        public Enemy(ClientMain client, UnitSnapshot snapshot) : base(client, snapshot)
        {
        }

        protected override void InnerUpdate()
        {
            base.InnerUpdate();

            CalcDir();
        }

        /// <summary>
        /// 根据与玩家的相对位置调整方向
        /// </summary>
        private void CalcDir()
        {
            var playerPos = Client.Player.Pos;
            var deltaPos = playerPos - this.Pos;
            var dot = FVector2.Dot(deltaPos, mMoveDir);
            if (dot > 0)
            {
                return;
            }

            if (FMath.Abs(deltaPos.x) >= FMath.Abs(deltaPos.y))
            {
                mMoveDir = deltaPos.x >= 0 ? FVector2.Right : FVector2.Left;
            }
            else
            {
                mMoveDir = deltaPos.y >= 0 ? FVector2.Up : FVector2.Down;
            }
        }

        public override void Spawn()
        {
            this.mMoveDir = FVector2.Zero;

            var playerPos = Client.Player.Pos;
            var dis = Const.MinSpawnDistance + Client.Player.Config.radius + this.Config.radius;
            var sqDistance = dis * dis;

            while (true)
            {
                var x = Client.NextRand() % (Const.MaxX * 2) - Const.MaxX;
                var y = Client.NextRand() % (Const.MaxY * 2) - Const.MaxY;

                var delta = new FVector2(x, y) - playerPos;
                if (delta.sqLength >= sqDistance)
                {
                    this.mCurPos = new FVector2(x, y);
                    return;
                }
            }
        }
    }
}