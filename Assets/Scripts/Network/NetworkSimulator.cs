using System;
using System.Collections.Generic;
using Log;
using Logic;

namespace Network
{
    /// <summary>
    /// 网络延时结构
    /// </summary>
    public class Latency
    {
        public float delay;
        public string data;

        public Latency(float delay, string data)
        {
            this.delay = delay;
            this.data = data;
        }
    }

    /// <summary>
    /// 模拟网络延时效果
    /// </summary>
    public class NetworkSimulator : INetwork
    {
        private ClientRecvFunc mClientFunc;
        private ServerRecvFunc mServerFunc;

        private List<Latency> mToClient;

        private List<Latency> mToServer;

        private Random mRand;
        private float mMinLatency, mMaxLatency;

        public void Init(ClientRecvFunc clientFunc, ServerRecvFunc serverFunc)
        {
            this.mClientFunc = clientFunc;
            this.mServerFunc = serverFunc;

            mToClient = new List<Latency>();
            mToServer = new List<Latency>();

            mRand = new Random(0);
        }

        public void SetLatency(float min, float max)
        {
            mMinLatency = min;
            mMaxLatency = max;
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
                if (latency.delay > deltaTime)
                {
                    latency.delay -= deltaTime;
                }
                else
                {
                    mToClient.RemoveAt(i);

                    //客户端接收到服务器下发
                    var frameOperation = latency.data.DeserializeFromString_PB<FrameOperation>();
                    mClientFunc(frameOperation);
                }
            }

            for (var i = mToServer.Count - 1; i >= 0; i--)
            {
                var latency = mToServer[i];
                if (latency.delay > deltaTime)
                {
                    latency.delay -= deltaTime;
                }
                else
                {
                    mToServer.RemoveAt(i);
                    // 服务器接收到用户输入
                    var frameOperation = latency.data.DeserializeFromString_PB<BaseOperation>();
                    mServerFunc(frameOperation);
                }
            }
        }

        public void SendToClient(FrameOperation frameOperation)
        {
            if (frameOperation.FrameIndex == -2)
            {
                Logger.Error("WTF");
            }
            var latency = new Latency(RandLatency(), frameOperation.SerializeToString_PB());
            mToClient.Add(latency);
        }

        public void SendToServer(BaseOperation operation)
        {
            var latency = new Latency(RandLatency(), operation.SerializeToString_PB());
            mToServer.Add(latency);
        }

        private float RandLatency()
        {
            var factor = (float)mRand.NextDouble();
            return mMinLatency + factor * (mMaxLatency - mMinLatency);
        }
    }
}