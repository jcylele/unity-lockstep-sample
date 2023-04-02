using System;
using System.Collections.Generic;
using Log;
using Logic;
using UnityEngine.UI;

namespace Panel
{
    /// <summary>
    /// used to configure some init params for gameplay
    /// </summary>
    public class BeginPanel : PanelSingleton<BeginPanel>
    {
        public Slider CountSlider;
        public Text CountTxt;

        public Slider LagSlider;
        public Text LagTxt;

        void OnEnable()
        {
            OnCountChanged();
            OnLagChanged();
        }

        public void OnCountChanged()
        {
            var count = (int)CountSlider.value;
            CountTxt.text = count.ToString();
        }

        public void OnLagChanged()
        {
            var lag = (int)LagSlider.value;
            LagTxt.text = lag.ToString();
        }

        public void OnBeginClicked()
        {
            var count = (int) (CountSlider.value);
            var enemy = new List<int>(count);
            for (var i = 0; i < count; i++)
            {
                enemy.Add(1);
            }

            var initInfo = new GameInitInfo
            {
                RandSeed = (uint) DateTimeOffset.Now.ToUnixTimeSeconds(),
                EnemySidList = enemy
            };

            var lag = LagSlider.value / 1000;

            GamePanel.Instance.StartGame(initInfo, lag);
        }
    }
}