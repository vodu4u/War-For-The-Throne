using DG.Tweening;
using System;
using UnityEngine;

namespace WarForTheThrone.Map
{
    public class Arrow : MonoBehaviour
    {
        public void Animate(Vector2 endPosition, bool direction, float duration, Action onEnd)
        {
            if (direction)
            {
                transform.localScale = Vector3.one;
            }
            else
            {
                transform.localScale = new Vector3(-1,1,1);
            }
            
            transform.DOMove(endPosition, duration).OnComplete(() => End(onEnd));
        }

        private void End(Action onEnd)
        {
            onEnd?.Invoke();
            gameObject.SetActive(false);
        }
    }
}