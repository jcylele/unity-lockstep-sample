using System;
using System.Collections.Generic;
using Log;
using Logic;

namespace Network
{
    /// <summary>
    /// simulate network latency
    /// </summary>
    public class Latency
    {
        public float Delay;
        public string Data;

        public Latency(float delay, string data)
        {
            this.Delay = delay;
            this.Data = data;
        }
    }

    /// <summary>
    /// simulate network lag, but no packet loss at present
    /// </summary>
    public class NetworkSimulator : INetwork
    {
        private ClientRecvFunc mClientFunc;
        private ServerRecvFunc mServerFunc;

        /// <summary>
        /// packets transporting towards client
        /// </summary>
        private List<Latency> mToClient;

        /// <summary>
        /// packets transporting towards server
        /// </summary>
        private List<Latency> mToServer;

        private Random mRand;
        /// <summary>
        /// average lag of the network
        /// </summary>
        private float mAverageLag;

        public void Init(ClientRecvFunc clientFunc, ServerRecvFunc serverFunc)
        {
            this.mClientFunc = clientFunc;
            this.mServerFunc = serverFunc;

            mToClient = new List<Latency>();
            mToServer = new List<Latency>();

            mRand = new Random(0);
        }

        /// <summary>
        /// set average lag
        /// </summary>
        /// <param name="lag"> average lag </param>
        public void SetAverageLag(float lag)
        {
            mAverageLag = lag;
        }

        public void Update(float deltaTime)
        {
            UpdateLatency(deltaTime);
        }

        private void UpdateLatency(float deltaTime)
        {
            for (var i = mToClient.Count - 1; i >= 0; i--)
            {
                var latency = mToClient[i];
                if (latency.Delay > deltaTime)
                {
                    latency.Delay -= deltaTime;
                }
                else
                {
                    mToClient.RemoveAt(i);

                    //packet arrives at client
                    var frameOperation = latency.Data.DeserializeFromString_PB<FrameData>();
                    mClientFunc(frameOperation);
                }
            }

            for (var i = mToServer.Count - 1; i >= 0; i--)
            {
                var latency = mToServer[i];
                if (latency.Delay > deltaTime)
                {
                    latency.Delay -= deltaTime;
                }
                else
                {
                    mToServer.RemoveAt(i);
                    // packet arrives at server
                    var frameOperation = latency.Data.DeserializeFromString_PB<BaseOperation>();
                    mServerFunc(frameOperation);
                }
            }
        }

        public void SendToClient(FrameData frameData)
        {
            if (frameData.FrameIndex == -2)
            {
                Logger.Error("WTF");
            }

            var latency = new Latency(RandomLag(), frameData.SerializeToString_PB());
            mToClient.Add(latency);
        }

        public void SendToServer(BaseOperation operation)
        {
            var latency = new Latency(RandomLag(), operation.SerializeToString_PB());
            mToServer.Add(latency);
        }

        /// <summary>
        /// randomize lag for the packet
        /// </summary>
        /// <returns></returns>
        private float RandomLag()
        {
            var factor = (float)mRand.NextDouble();
            return factor * mAverageLag * 2;
        }
    }
}