using FP;

namespace Logic
{
    /// <summary>
    /// logical enemy unit
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
        /// calculate move direction to player
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
            var dis = LogicConst.MinSpawnDistance + Client.Player.Config.radius + this.Config.radius;
            var sqDistance = dis * dis;

            // spawn at a random position that is far enough from player
            while (true)
            {
                var x = Client.NextRand() % (LogicConst.MaxX * 2) - LogicConst.MaxX;
                var y = Client.NextRand() % (LogicConst.MaxY * 2) - LogicConst.MaxY;

                var delta = new FVector2(x, y) - playerPos;
                if (delta.SqLength >= sqDistance)
                {
                    this.mCurPos = new FVector2(x, y);
                    return;
                }
            }
        }
    }
}