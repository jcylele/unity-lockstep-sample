using FixMath;
using Log;

namespace Logic
{
    /// <summary>
    /// 逻辑作战单位(玩家，敌人的基类)
    /// </summary>
    public class  Unit : RecordableObject<UnitRecord>, ISnapshot<UnitSnapshot>
    {
        public int Sid { get; private set; }

        public UnitConfig Config { get; private set; }

        protected FVector2 mMoveDir;
        public FVector2 MoveDir => mMoveDir;

        protected FVector2 mCurPos;
        public FVector2 Pos => mCurPos;

        public Unit(ClientMain client, int sid) : base(client)
        {
            Sid = sid;
            Config = UnitConfig.GetConfig(sid);
        }

        public Unit(ClientMain client, UnitSnapshot snapshot) : base(client, snapshot)
        {
            this.RevertToSnapShot(snapshot, false);
        }

        protected override void InnerUpdate()
        {
            base.InnerUpdate();

            if (mMoveDir == FVector2.Zero) return;

            Logger.Assert(mMoveDir.sqLength == FPoint.One, "mMoveDir not normalized");

            mCurPos += mMoveDir * Config.moveSpeed * Const.FrameInterval;

            if (mCurPos.x > Const.MaxX)
                mCurPos.x = Const.MaxX;
            else if (mCurPos.x < -Const.MaxX) mCurPos.x = -Const.MaxX;

            if (mCurPos.y > Const.MaxY)
                mCurPos.y = Const.MaxY;
            else if (mCurPos.y < -Const.MaxY) mCurPos.y = -Const.MaxY;
        }

        public virtual void Spawn()
        {
        }

        protected override void CaptureRecord(UnitRecord record)
        {
            base.CaptureRecord(record);

            record.Pos = Pos;
            record.MoveDir = MoveDir;
        }

        public void SaveToSnapShot(UnitSnapshot snapshot)
        {
            base.SaveToSnapShot(snapshot);

            snapshot.Sid = Sid;
            snapshot.MoveDir = MoveDir;
            snapshot.Pos = Pos;
        }

        public void RevertToSnapShot(UnitSnapshot snapshot, bool needBase)
        {
            if (needBase)
            {
                base.RevertToSnapShot(snapshot, true);
            }
            this.Sid = snapshot.Sid;
            Config = UnitConfig.GetConfig(Sid);

            this.mMoveDir = snapshot.MoveDir;
            this.mCurPos = snapshot.Pos;
        }
    }
}