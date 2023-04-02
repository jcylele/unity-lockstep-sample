using Logic;
using System.Collections.Generic;
using UnityEngine;

namespace Mono
{
    /// <summary>
    /// show unit of player
    /// </summary>
    public class PlayerBehaviour : MonoBehaviour
    {
        private static readonly Dictionary<KeyCode, KeyType> KeyConvertMap = new Dictionary<KeyCode, KeyType>
        {
            { KeyCode.UpArrow, KeyType.Up },
            { KeyCode.DownArrow, KeyType.Down },
            { KeyCode.LeftArrow, KeyType.Left },
            { KeyCode.RightArrow, KeyType.Right },
        };

        public Player Player { set; get; }

        private Vector2 mOldPos, mNewPos;

        void Start()
        {
            var rt = this.GetComponent<RectTransform>();
            rt.sizeDelta = Vector2.one * Player.Config.radius * 2;
        }

        void Update()
        {
            ProcessInput();
            UpdateMove();
        }

        private void ProcessInput()
        {
            foreach (var pair in KeyConvertMap)
            {
                if (Input.GetKeyDown(pair.Key))
                {
                    Player.Client.PlayerInputManager.SendKeyOperation(pair.Value, KeyOpType.Pressed);
                }
                else if (Input.GetKeyUp(pair.Key))
                {
                    Player.Client.PlayerInputManager.SendKeyOperation(pair.Value, KeyOpType.Released);
                }
            }
        }

        private void UpdateMove()
        {
            mNewPos = Player.Pos.ToVector2();
            mOldPos = this.transform.localPosition;

            var deltaPos = mNewPos - mOldPos;
            
            this.transform.localRotation = Quaternion.FromToRotation(Vector3.up, Player.MoveDir.ToVector2());

            // lerp current show position and logical position
            var pos = Vector2.Lerp(mOldPos, mNewPos, 0.5f);

            this.transform.localPosition = pos;
        }
    }
}