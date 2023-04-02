using System.Collections.Generic;
using FP;
using ProtoBuf;

namespace Logic
{
    /// <summary>
    /// operation type in game play
    /// </summary>
    public enum OperationType
    {
        None,

        /// <summary>
        /// client to server(C2S), client is ready to start the game
        /// </summary>
        GameReady,

        /// <summary>
        /// server to client(S2C), game starts
        /// </summary>
        GameStart,

        /// <summary>
        /// client to server(C2S), game over
        /// </summary>
        GameFinish,

        /// <summary>
        /// special operation,
        /// operations above this is special,
        /// while operations below this is normal.
        /// </summary>
        MinNormal = 100,

        /// <summary>
        /// Keyboard Input from user
        /// </summary>
        Key,
    }

    /// <summary>
    /// initial info of a game
    /// </summary>
    [ProtoContract]
    public class GameInitInfo
    {
        [ProtoMember(1)] public ulong RandSeed;
        [ProtoMember(2)] public List<int> EnemySidList;
    }

    /// <summary>
    /// report of a game, can be used to replay a game
    /// </summary>
    [ProtoContract]
    public class GameReport
    {
        [ProtoMember(1)] public GameInitInfo GameInitInfo;
        [ProtoMember(2)] public List<FrameData> FrameOperationList;
    }

    /// <summary>
    /// base class of all single operations
    /// </summary>
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

    /// <summary>
    /// whole data of a frame, contains frame index and all operations in this frame
    /// </summary>
    [ProtoContract]
    public class FrameData
    {
        [ProtoMember(1)] public int FrameIndex;
        [ProtoMember(2)] public List<BaseOperation> OperationList;

        public FrameData() : this(0, null)
        {
        }

        public FrameData(int frameIndex, List<BaseOperation> operationList)
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

    /// <summary>
    /// result info of the game,records specific info of the game,
    /// used to validate the game, such checking for cheating and bugs
    /// </summary>
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