using System;
using System.Collections.Generic;
using Log;
using Logic;
using UnityEngine.UI;

namespace Panel
{
    public class ReplayPanel : PanelSingleton<ReplayPanel>
    {
        public Dropdown ReportDropdown;
        private List<long> mReportKeys;

        void OnEnable()
        {
            mReportKeys = DataBase.Instance.GetReportKeys();
            ReportDropdown.ClearOptions();
            var list = new List<string>(mReportKeys.Count);
            foreach (var timeStamp in mReportKeys)
            {
                var dto = DateTimeOffset.FromUnixTimeSeconds(timeStamp);
                list.Add(dto.LocalDateTime.ToString("G"));
            }

            ReportDropdown.AddOptions(list);
        }

        public void OnReplayClicked()
        {
            var index = ReportDropdown.value;
            if (index < 0 ||  index  >= mReportKeys.Count)
            {
                Logger.Error("Wrong Report");
                return;
            }
            var report = DataBase.Instance.GetReport(mReportKeys[index]);
            GamePanel.Instance.StartReplay(report);
        }
    }
}