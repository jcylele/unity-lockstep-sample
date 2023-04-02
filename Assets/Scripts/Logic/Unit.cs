using FP;
using Log;

namespace Logic
{
    /// <summary>
    /// base class for all units
    /// </summary>
    public class Unit : ClientObject, ISnapshot<UnitSnapshot>
    {
        public int Sid { get; private set; }

        public UnitConfig Config { get; private set; }

        protected FVector2 mMoveDir;
        public FVector2 MoveDir => mMoveDir;

        protected FVector2 mCurPos;
        public FVector2 Pos => mCurPos;

        /// <summary>
        /// constructor for new unit
        /// </summary>
        public Unit(ClientMain client, int sid) : base(client)
        {
            Sid = sid;
            Config = UnitConfig.GetConfig(sid);
        }

        /// <summary>
        /// constructor for snapshot
        /// </summary>
        public Unit(ClientMain client, UnitSnapshot snapshot) : base(client, snapshot)
        {
            this.RevertFromSnapShot(snapshot);
        }

        protected override void InnerUpdate()
        {
            base.InnerUpdate();

            if (mMoveDir == FVector2.Zero) return;

            Logger.Assert(mMoveDir.SqLength == FPoint.One, "mMoveDir not normalized");

            mCurPos += mMoveDir * Config.moveSpeed * LogicConst.FrameInterval;

            if (mCurPos.x > LogicConst.MaxX)
                mCurPos.x = LogicConst.MaxX;
            else if (mCurPos.x < -LogicConst.MaxX) mCurPos.x = -LogicConst.MaxX;

            if (mCurPos.y > LogicConst.MaxY)
                mCurPos.y = LogicConst.MaxY;
            else if (mCurPos.y < -LogicConst.MaxY) mCurPos.y = -LogicConst.MaxY;
        }

        public virtual void Spawn()
        {
        }

        #region snapshot
        
        public void SaveToSnapShot(UnitSnapshot snapshot)
        {
            base.SaveToSnapShot(snapshot);

            snapshot.Sid = Sid;
            snapshot.MoveDir = MoveDir;
            snapshot.Pos = Pos;
        }

        public void RevertFromSnapShot(UnitSnapshot snapshot)
        {
            base.RevertFromSnapShot(snapshot);
            this.Sid = snapshot.Sid;
            Config = UnitConfig.GetConfig(Sid);

            this.mMoveDir = snapshot.MoveDir;
            this.mCurPos = snapshot.Pos;
        }
        
        #endregion
    }
}