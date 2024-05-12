using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WarForTheThrone.UI
{

    public class PlayerUI : MonoBehaviour
    {
        private int _score = 0;
        private string _name;

        [SerializeField] private Image _imgPlayerFill; 
        [SerializeField] private TextMeshProUGUI _txtName;
        [SerializeField] private TextMeshProUGUI _txtScore;
        [SerializeField] private GameObject _playerTurnGO;

        public string Name 
        { get => _name; 
            set 
            { 
                _name = value;
                _txtName.text = _name;
            }
        }
        public int Score
        {
            get => _score;
            set
            {
                _score = value;
                _txtScore.text = "Ñ÷¸ò: " + _score;
            }
        }

        public void SetTurn(bool state)
        {
            _playerTurnGO.SetActive(state);
        }

        public void UpdateUnitsAlive(int count, int maxUnits)
        {
            _imgPlayerFill.DOFillAmount((float)count / maxUnits, 1f);
        }
    }
}