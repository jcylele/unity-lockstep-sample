using System.Collections.Generic;
using FixMath;
using ProtoBuf;

namespace Logic
{
    public interface ISnapshot<in T> where T : BaseSnapshot
    {
        void SaveToSnapShot(T snapshot);
        void RevertToSnapShot(T snapshot, bool needBase);
    }

    [ProtoContract]
    [ProtoInclude(101, typeof(ClientSnapshot))]
    [ProtoInclude(102, typeof(ClientObjectSnapshot))]
    [ProtoInclude(103, typeof(StaticSnapshot))]
    [ProtoInclude(104, typeof(FRandomSnapshot))]
    public class BaseSnapshot
    {
    }

    [ProtoContract]
    public class StaticSnapshot : BaseSnapshot
    {
        [ProtoMember(1)]
        public ulong ClientObject_NextUid;
    }
    [ProtoContract]
    public class FRandomSnapshot : BaseSnapshot
    {
        [ProtoMember(1)]
        public ulong CurSeed;
    }

    [ProtoContract]
    [ProtoInclude(201, typeof(UnitSnapshot))]
    [ProtoInclude(202, typeof(UnitManagerSnapshot))]
    public class ClientObjectSnapshot : BaseSnapshot
    {
        [ProtoMember(1)]
        public ulong Uid;
    }

    [ProtoContract]
    public class UnitSnapshot : ClientObjectSnapshot
    {
        [ProtoMember(1)]
        public int Sid;
        [ProtoMember(2)]
        public FVector2 MoveDir;
        [ProtoMember(3)]
        public FVector2 Pos;
    }

    [ProtoContract]
    public class UnitManagerSnapshot : ClientObjectSnapshot
    {
        [ProtoMember(1)]
        public UnitSnapshot Player;
        [ProtoMember(2)]
        public List<UnitSnapshot> EnemyList;

        public UnitManagerSnapshot()
        {
            Player = new UnitSnapshot();
            EnemyList = new List<UnitSnapshot>();
        }
    }

    [ProtoContract]
    public class ClientSnapshot : BaseSnapshot
    {
        [ProtoMember(1)]
        public StaticSnapshot Static;
        [ProtoMember(2)]
        public int CurFrame;
        [ProtoMember(3)]
        public FRandomSnapshot Rand;
        [ProtoMember(4)]
        public UnitManagerSnapshot UnitMgr;

        public ClientSnapshot()
        {
            Static = new StaticSnapshot();
            CurFrame = 0;
            Rand = new FRandomSnapshot();
            UnitMgr = new UnitManagerSnapshot();
        }
    }
}