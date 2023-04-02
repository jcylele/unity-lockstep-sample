using System.Collections.Generic;
using FP;
using ProtoBuf;

namespace Logic
{
    /// <summary>
    /// interface for snapshot,
    /// <para>all classes that need to be saved to and reverted from snapshot should implement this interface</para>
    /// <para>used for rollback(not implemented yet), fast reconnect and consistency check</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISnapshot<in T> where T : BaseSnapshot
    {
        /// <summary>
        /// save current state to snapshot
        /// </summary>
        /// <param name="snapshot">target snapshot</param>
        void SaveToSnapShot(T snapshot);
        /// <summary>
        /// revert to specific state from snapshot
        /// </summary>
        /// <param name="snapshot">source snapshot</param>
        void RevertFromSnapShot(T snapshot);
    }

    /// <summary>
    /// base class to store snapshot data
    /// </summary>
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
        public ulong  CurSeed;
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