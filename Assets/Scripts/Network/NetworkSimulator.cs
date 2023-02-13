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
        public float Delay;
        public string Data;

        public Latency(float delay, string data)
        {
            this.Delay = delay;
            this.Data = data;
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
        private float mAverageLag;

        public void Init(ClientRecvFunc clientFunc, ServerRecvFunc serverFunc)
        {
            this.mClientFunc = clientFunc;
            this.mServerFunc = serverFunc;

            mToClient = new List<Latency>();
            mToServer = new List<Latency>();

            mRand = new Random(0);
        }

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

                    //客户端接收到服务器下发
                    var frameOperation = latency.Data.DeserializeFromString_PB<FrameOperation>();
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
                    // 服务器接收到用户输入
                    var frameOperation = latency.Data.DeserializeFromString_PB<BaseOperation>();
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
            var latency = new Latency(RandomLag(), frameOperation.SerializeToString_PB());
            mToClient.Add(latency);
        }

        public void SendToServer(BaseOperation operation)
        {
            var latency = new Latency(RandomLag(), operation.SerializeToString_PB());
            mToServer.Add(latency);
        }

        private float RandomLag()
        {
            var factor = (float)mRand.NextDouble();
            return factor * mAverageLag * 2;
        }
    }
}