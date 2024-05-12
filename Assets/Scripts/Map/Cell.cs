using UnityEngine;
using UnityEngine.UI;

namespace WarForTheThrone.Map
{
    public class Cell : MonoBehaviour
    {
        [SerializeField] private Image _image;
        private bool _isAvaible;
        public int X { get; private set; }
        public int Y { get; private set; }
        public bool HoldCustomColor { get; set; }
        public bool IsAvaible { get => _isAvaible; set
            {
                _isAvaible = value;
                ResetColor();
            }
        }
        [field: SerializeField] public float Size { get; set; }

        public void Initialize(int x, int y)
        {
            IsAvaible = false;
            X = x;
            Y = y;
        }

        public void SetCustomColor(Color color)
        {
            _image.color = color;
        }

        public void ResetColor()
        {
            if(!HoldCustomColor)
                _image.color = IsAvaible ? Color.white : Color.black;
        }
    }
}