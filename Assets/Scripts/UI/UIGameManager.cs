using DG.Tweening;
using WarForTheThrone.Info;
using WarForTheThrone.Map;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WarForTheThrone.UI
{
    public class UIGameManager : MonoBehaviour
    {
        private int _timer = 0;
        private bool _timerActive = false;
        [SerializeField] private GameObject _timerGO;
        [SerializeField] private TextMeshProUGUI _txtTimer;
        [SerializeField] private UnitPlacer _placer;
        [SerializeField] private GameConfigSO _gameInfo;
        [SerializeField] private Button[] _butChooseUnit;
        [SerializeField] private TextMeshProUGUI _txtUnitInfo;
        [SerializeField] private TextMeshProUGUI _txtChoosedUnit;
        [SerializeField] private Image _imgUnitPreview;
        [SerializeField] private PlayerUI[] _playerUIs;
        [SerializeField] private GameObject _fightLayoutGO;
        [SerializeField] private GameObject _chooseUnitsLayoutGO;
        [SerializeField] private GameObject _pause;
        [SerializeField] private TextMeshProUGUI _txtPlayerWin;
        [SerializeField] private GameObject _playerWin;

        public enum Layout
        {
            PlaceUnits,
            Fight
        }

        private void Start()
        {
            _playerUIs[0].SetTurn(false);
            _playerUIs[1].SetTurn(false);
            ChooseUnit(0);
        }

        private IEnumerator UpdateTimer()
        {
            while (_timerActive)
            {
                TimeSpan timeSpan = TimeSpan.FromSeconds(_timer);
                _txtTimer.text = string.Format("{0:00}:{1:00}", timeSpan.TotalMinutes, timeSpan.Seconds);
                yield return new WaitForSeconds(1f);
                _timer++;
            }
        }

        public void ChooseUnit(int id)
        {
            for (int i = 0; i < _butChooseUnit.Length; i++)
            {
                if(i == id)
                {
                    _butChooseUnit[i].transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), .25f);
                }
                else
                {
                    _butChooseUnit[i].transform.DOScale(new Vector3(.75f, .75f, .75f), .25f);
                }
                _butChooseUnit[i].interactable = i != id;
            }

            UnitConfigSO unitSO = _gameInfo.Units[id];
            _txtUnitInfo.text = "<b>" + unitSO.Name + Environment.NewLine +
                "</b> Здоровье: " + unitSO.Health + Environment.NewLine +
                "Атака: " + unitSO.Damage + Environment.NewLine +
                "Радиус атаки: " + unitSO.AttackRange;

            _txtChoosedUnit.text = "Выбран юнит: <b>" + unitSO.Name;
            _imgUnitPreview.sprite = unitSO.Preview;
            _placer.Unit = unitSO;
        }

        public void UpdateUnitsAlive(int player, int alive, int max)
        {
            _playerUIs[player].UpdateUnitsAlive(alive, max);
        }
        public void SetLayout(Layout layout)
        {
            _fightLayoutGO.SetActive(layout == Layout.Fight);
            _chooseUnitsLayoutGO.SetActive(layout == Layout.PlaceUnits);
        }
        public void SetPlayerWin(int player)
        {
            _txtPlayerWin.text = "Победил игрок " + _playerUIs[player].Name + " со счетом " + _playerUIs[player].Score;
            _playerWin.SetActive(true);
            SetTimer(false);
        }

        public void SetTimer(bool state)
        {
            _timerGO.SetActive(state);
            _timerActive = state;
            StartCoroutine(nameof(UpdateTimer));
        }

        public void ChangeTurn(int turn)
        {
            _playerUIs[0].SetTurn(turn == 0);
            _playerUIs[1].SetTurn(turn == 1);
        }

        public void AddScore(int score, int player)
        {
            _playerUIs[player].Score += score;
        }

        public void SetPause(bool state)
        {
            _pause.SetActive(state);
            Time.timeScale = state ? 0 : 1;
        }
    }
}