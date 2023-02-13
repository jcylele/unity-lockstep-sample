using System.Collections.Generic;

namespace Logic
{
    public class RecordableObject<T> : ClientObject where T : BaseRecord, new()
    {
        // private readonly Stack<T> mCache = new Stack<T>();
        private readonly Queue<T> mRecords = new Queue<T>();

        public RecordableObject(ClientMain client) : base(client)
        {
        }

        public RecordableObject(ClientMain client, ClientObjectSnapshot snapshot) : base(client, snapshot)
        {
        }

        public sealed override void LogicUpdate()
        {
            InnerUpdate();

            if (Client.WithRecord)
            {
                var record = new T();
                CaptureRecord(record);
                mRecords.Enqueue(record);
            }
        }

        // private T NewRecord()
        // {
        //     if (mCache.Count > 0)
        //     {
        //         return mCache.Pop();
        //     }
        //
        //     return new T();
        // }

        public T PopRecord()
        {
            if (mRecords.Count == 0)
            {
                return null;
            }

            return mRecords.Dequeue();
        }

        protected virtual void CaptureRecord(T record)
        {
            record.Frame = Client.CurFrame;
        }
    }
}