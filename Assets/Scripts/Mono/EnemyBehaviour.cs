using FP;
using Logic;
using UnityEngine;

namespace Mono
{
    /// <summary>
    /// show unit of enemy
    /// </summary>
    public class EnemyBehaviour : MonoBehaviour
    {
        public Enemy Enemy { set; get; }

        void Start()
        {
            var rt = this.GetComponent<RectTransform>();
            rt.sizeDelta = Vector2.one * Enemy.Config.radius * 2;
        }

        void Update()
        {
            UpdateMove();
        }

        private void UpdateMove()
        {
            this.transform.localPosition = Enemy.Pos.ToVector2();

            if (Enemy.MoveDir != FVector2.Zero)
            {
                var rotation = Quaternion.FromToRotation(Vector3.up, Enemy.MoveDir.ToVector2());
                this.transform.localRotation = rotation;
            }
        }
    }
}