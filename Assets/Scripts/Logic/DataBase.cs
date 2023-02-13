using System;
using System.Collections.Generic;

namespace Logic
{
    public class DataBase : Singleton<DataBase>
    {
        private readonly SortedDictionary<long, byte[]> mReports = new SortedDictionary<long, byte[]>();

        public void SaveReport(GameReport report)
        {
            var bytes = report.SerializeToByteAry_PB();
            var timeStamp = DateTimeOffset.Now.ToUnixTimeSeconds(); // 相差秒数
            mReports.Add(timeStamp, bytes);
        }

        public GameReport GetReport(long timeStamp)
        {
            mReports.TryGetValue(timeStamp, out var bytes);
            return bytes.DeserializeFromByteAry_PB<GameReport>();
        }

        public List<long> GetReportKeys()
        {
            return new List<long>(mReports.Keys);
        }
    }
}