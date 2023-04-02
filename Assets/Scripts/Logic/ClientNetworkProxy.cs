using Log;
using Network;
using System.Collections.Generic;

namespace Logic
{
    /// <summary>
    /// handler for frame data sending and receiving
    /// </summary>
    public class ClientNetworkProxy
    {
        /// <summary>
        /// network interface
        /// </summary>
        private readonly INetwork mNetwork;

        /// <summary>
        /// minimal frame index, data with smaller frame index will be dropped
        /// </summary>
        private int mMinFrameIndex;

        /// <summary>
        /// frame operation wait to be processed
        /// </summary>
        private readonly LinkedList<FrameData> mWaitFrames = new LinkedList<FrameData>();

        public delegate void SpecialFrameOperationHandler(FrameData frameData);
        /// <summary>
        /// outer handler for special frame operation
        /// </summary>
        public event SpecialFrameOperationHandler SpFrameHandler;
        
        public ClientNetworkProxy(INetwork network)
        {
            mNetwork = network;
        }

        /// <summary>
        /// get frame operation by frame index,
        /// if succeed, set the frame index to mMinFrameIndex,
        /// then smaller frame index will be dropped
        /// </summary>
        public FrameData GetFrameOperation(int frameIndex)
        {
            mMinFrameIndex = frameIndex;

            while (mWaitFrames.Count > 0)
            {
                var fo = mWaitFrames.First.Value;
                if (fo.FrameIndex < frameIndex)
                {
                    mWaitFrames.RemoveFirst();
                }
                else if (fo.FrameIndex == frameIndex)
                {
                    mWaitFrames.RemoveFirst();
                    mMinFrameIndex = frameIndex + 1;
                    return fo;
                }
                else
                {
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// send operation to server
        /// </summary>
        public void SendToServer(BaseOperation operation)
        {
            // Logger.Assert(operation.OpType > OperationType.MinNormal,
            //     $"${operation.OpType} Not Allowed Here, should greater than OperationType.MinNormal");
            Logger.Info($"[Frame] Client Send {operation}");
            mNetwork?.SendToServer(operation);
        }

        /// <summary>
        /// receive frame data from server
        /// </summary>
        public void OnReceiveFrameData(FrameData frameData)
        {
            switch (frameData.FrameIndex)
            {
                case -1: //special frame operation
                {
                    Logger.Info($"[Frame] Client Receive {frameData}");
                    
                    this.SpFrameHandler?.Invoke(frameData);
                }
                    break;
                default:
                {
                    //drop older frame data
                    if (frameData.FrameIndex < mMinFrameIndex)
                    {
                        return;
                    }
                    if (frameData.OperationList != null)
                    {
                        Logger.Info($"[Frame] Client Receive {frameData}");
                    }
                    //add to appropriate position(sorted by frame index)
                    for (var node = mWaitFrames.Last; node != null; node = node.Previous)
                    {
                        var df = frameData.FrameIndex - node.Value.FrameIndex;
                        if (df == 0)
                        {
                            //reduplicate, skip
                            return;
                        }
                        else if (df > 0)
                        {
                            mWaitFrames.AddAfter(node, frameData);
                            return;
                        }
                    }

                    //empty linked list or new minimal frame index
                    mWaitFrames.AddFirst(frameData);
                }
                    break;
            }
        }
    }
}