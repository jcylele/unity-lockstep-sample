using System.Collections.Generic;
using FixMath;
using ProtoBuf;

namespace Logic
{
    /// <summary>
    /// 帧操作
    /// </summary>
    public enum OperationType
    {
        None,

        /// <summary>
        /// 准备就绪，参见<see cref="GameReadyOperation"/>
        /// </summary>
        GameReady,

        /// <summary>
        /// 战斗开始，参见<see cref="GameStartOperation"/>
        /// </summary>
        GameStart,

        /// <summary>
        /// 战斗结束，参见<see cref="GameFinishOperation"/>
        /// </summary>
        GameFinish,

        /// <summary>
        /// 特殊标志，特殊帧和普通帧的分割线
        /// </summary>
        MinNormal = 100,

        /// <summary>
        /// 按键，参见<see cref="KeyOperation"/>
        /// </summary>
        Key,
    }

    [ProtoContract]
    public class GameInitInfo
    {
        [ProtoMember(1)] public ulong RandSeed;
        [ProtoMember(2)] public List<int> EnemySidList;
    }

    [ProtoContract]
    public class GameReport
    {
        [ProtoMember(1)] public GameInitInfo GameInitInfo;
        [ProtoMember(2)] public List<FrameOperation> FrameOperationList;
    }

    [ProtoContract]
    [ProtoInclude(101, typeof(KeyOperation))]
    [ProtoInclude(102, typeof(GameReadyOperation))]
    [ProtoInclude(103, typeof(GameStartOperation))]
    [ProtoInclude(104, typeof(GameFinishOperation))]
    public class BaseOperation
    {
        [ProtoMember(1)] public readonly OperationType OpType;

        public BaseOperation(OperationType opType = OperationType.None)
        {
            this.OpType = opType;
        }

        public override string ToString()
        {
            return $"({OpType})";
        }
    }

    [ProtoContract]
    public class FrameOperation
    {
        [ProtoMember(1)] public int FrameIndex;
        [ProtoMember(2)] public List<BaseOperation> OperationList;

        public FrameOperation() : this(0, null)
        {
        }

        public FrameOperation(int frameIndex, List<BaseOperation> operationList)
        {
            this.FrameIndex = frameIndex;
            this.OperationList = operationList;
        }

        public override string ToString()
        {
            string strList;
            if (OperationList == null)
            {
                strList = "null";
            }
            else
            {
                var list = new List<string>(OperationList.Count);
                foreach (var operation in OperationList)
                {
                    list.Add(operation.ToString());
                }

                strList = string.Join(", ", list);
            }

            return $"[{FrameIndex}:{strList}]";
        }
    }

    [ProtoContract]
    public class GameReadyOperation : BaseOperation
    {
        public GameReadyOperation() : base(OperationType.GameReady)
        {
        }
    }

    [ProtoContract]
    public class GameStartOperation : BaseOperation
    {
        public GameStartOperation() : base(OperationType.GameStart)
        {
        }
    }

    [ProtoContract]
    public class ResultInfo
    {
        [ProtoMember(1)] public int LastFrame;
        [ProtoMember(2)] public List<FVector2> PosList;

        public static bool operator !=(ResultInfo result1, ResultInfo result2)
        {
            return !(result1 == result2);
        }

        public static bool operator ==(ResultInfo result1, ResultInfo result2)
        {
            if (ReferenceEquals(result1, result2))
            {
                return true;
            }

            if (ReferenceEquals(result1, null) || ReferenceEquals(result2, null))
            {
                return false;
            }

            if (result1.LastFrame != result2.LastFrame)
            {
                return false;
            }

            if (result1.PosList == result2.PosList)
            {
                return true;
            }

            if (result1.PosList == null || result2.PosList == null)
            {
                return false;
            }

            if (result1.PosList.Count != result2.PosList.Count)
            {
                return false;
            }

            for (int i = 0; i < result1.PosList.Count; i++)
            {
                if (result1.PosList[i] != result2.PosList[i])
                {
                    return false;
                }
            }

            return true;
        }
    }

    [ProtoContract]
    public class GameFinishOperation : BaseOperation
    {
        [ProtoMember(1)] public readonly ResultInfo Result;

        public GameFinishOperation() : this(null)
        {
        }

        public GameFinishOperation(ResultInfo result) : base(OperationType.GameFinish)
        {
            this.Result = result;
        }

        public override string ToString()
        {
            return $"({OpType}, {Result.LastFrame})";
        }
    }

    public enum KeyType
    {
        None,
        Up,
        Down,
        Left,
        Right
    }

    public enum KeyOpType
    {
        None,
        Pressed,
        Released,
    }

    [ProtoContract]
    public class KeyOperation : BaseOperation
    {
        [ProtoMember(1)] public KeyType Key;
        [ProtoMember(2)] public KeyOpType KeyOpType;

        public KeyOperation(KeyType key, KeyOpType keyOpType) : base(OperationType.Key)
        {
            this.Key = key;
            this.KeyOpType = keyOpType;
        }

        public KeyOperation() : this(KeyType.None, KeyOpType.None)
        {
        }

        public override string ToString()
        {
            return $"({OpType}, {Key} {KeyOpType})";
        }
    }
}