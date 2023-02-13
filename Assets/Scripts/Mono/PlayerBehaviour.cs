using System;
using Logic;
using System.Collections.Generic;
using FixMath;
using Unity.Mathematics;
using UnityEngine;

namespace Mono
{
    /// <summary>
    /// 表现玩家
    /// </summary>
    public class PlayerBehaviour : MonoBehaviour
    {
        private static readonly Dictionary<KeyCode, KeyType> KeyConvertMap = new Dictionary<KeyCode, KeyType>
        {
            {KeyCode.UpArrow, KeyType.Up},
            {KeyCode.DownArrow, KeyType.Down},
            {KeyCode.LeftArrow, KeyType.Left},
            {KeyCode.RightArrow, KeyType.Right},
        };

        public Player Player { set; get; }

        private Vector2 mOldPos, mNewPos;
        private float mElapsedTime;

        void Start()
        {
            var rt = this.GetComponent<RectTransform>();
            rt.sizeDelta = Vector2.one * Player.Config.radius * 2;
        }

        void Update()
        {
            UpdateInput();
            UpdateMove();
        }

        private void UpdateInput()
        {
            foreach (var pair in KeyConvertMap)
            {
                if (Input.GetKeyDown(pair.Key))
                {
                    Player.SendOperation(pair.Value, KeyOpType.Pressed);
                }
                else if (Input.GetKeyUp(pair.Key))
                {
                    Player.SendOperation(pair.Value, KeyOpType.Released);
                }
            }
        }

        private void UpdateMove()
        {
            var record = Player.PopRecord();
            if (record != null)
            {
                mNewPos = record.Pos.ToVector2();
                mOldPos = this.transform.localPosition;
                mElapsedTime = 0f;

                var deltaPos = mNewPos - mOldPos;
                this.transform.localRotation = Quaternion.FromToRotation(Vector3.up, deltaPos);
            }

            mElapsedTime += Time.deltaTime;
            var rate = math.clamp(mElapsedTime / Const.FrameInterval, 0, 1);
            var pos = (1 - rate) * mOldPos + rate * mNewPos;

            this.transform.localPosition = pos;
        }
    }
}