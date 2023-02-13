using System;
using System.Collections.Generic;
using Log;
using Logic;
using UnityEngine.UI;

namespace Panel
{
    public class BeginPanel : PanelSingleton<BeginPanel>
    {
        public Slider CountSlider;
        public Text CountTxt;

        void OnEnable()
        {
            OnCountChanged();
        }

        public void OnCountChanged()
        {
            var count = (int)CountSlider.value;
            CountTxt.text = count.ToString();
        }

        public void OnBeginClicked()
        {
            var count = (int) (CountSlider.value);
            var enemy = new List<int>(count);
            for (var i = 0; i < count; i++)
            {
                enemy.Add(1);
            }

            var initInfo = new InitInfo
            {
                RandSeed = (uint) DateTimeOffset.Now.ToUnixTimeSeconds(),
                EnemySidList = enemy
            };

            GamePanel.Instance.StartGame(initInfo);
        }
    }
}