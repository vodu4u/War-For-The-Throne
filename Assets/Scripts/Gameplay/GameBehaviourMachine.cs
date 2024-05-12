using UnityEngine;
using KevinCastejon.FiniteStateMachine;
using WarForTheThrone.Map;
using WarForTheThrone.UI;
using TMPro;
using UnityEngine.SceneManagement;
namespace WarForTheThrone.Gameplay
{
    public class GameBehaviourMachine : AbstractFiniteStateMachine
    {
        [SerializeField] private UnitPlacer _unitPlacer;
        [SerializeField] private UIGameManager _uiManager;
        [SerializeField] private UnitController _unitController;
        [SerializeField] private CellManager _cellManagement;
        [SerializeField] private TMP_InputField[] _inputPlayers;
        [SerializeField] private PlayerUI[] _playerUIs;
        [SerializeField] private GameObject _inputPlayer;
        public int PlayerWinID { get;  set; }

        public enum GameplayState
        {
            INPUT_NAME,
            UNIT_PLACEMENT,
            UNIT_MANAGEMENT,
            GAME_OVER
        }
        private void Awake()
        {
            Init(GameplayState.INPUT_NAME,
                AbstractState.Create<InputNameState, GameplayState>(GameplayState.INPUT_NAME, this),
                AbstractState.Create<UnitPlacementState, GameplayState>(GameplayState.UNIT_PLACEMENT, this),
                AbstractState.Create<UnitManagementState, GameplayState>(GameplayState.UNIT_MANAGEMENT, this),
                AbstractState.Create<GameOverState, GameplayState>(GameplayState.GAME_OVER, this)
            );
        }

        public void StartGame()
        {
            for (int i = 0; i < _playerUIs.Length; i++)
            {
                if (_inputPlayers[i].text == "")
                    _playerUIs[i].Name = "Игрок " + (i + 1).ToString();
                else
                    _playerUIs[i].Name = _inputPlayers[i].text;
            }
            _inputPlayer.SetActive(false);
            TransitionToState(GameplayState.UNIT_PLACEMENT);
        }

        public void LoadMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("menu");
        }

        public class InputNameState : AbstractState
        {
            private GameBehaviourMachine _machine { get => _parentStateMachine as GameBehaviourMachine; }
            public override void OnEnter()
            {
                _machine._inputPlayer.SetActive(true);
            }
            public override void OnExit()
            {
                _machine._inputPlayer.SetActive(false);
            }
        }

        public class UnitPlacementState : AbstractState
        {
            private GameBehaviourMachine _machine { get => _parentStateMachine as GameBehaviourMachine; }

            public override void OnEnter()
            {
                _machine._unitPlacer.IsActive = true;
                _machine._uiManager.SetLayout(UIGameManager.Layout.PlaceUnits);
                _machine._unitPlacer.Initialize(() => TransitionToState(GameplayState.UNIT_MANAGEMENT));
            }
            public override void OnExit()
            {
                _machine._unitPlacer.IsActive = false;
            }
        }
        public class UnitManagementState : AbstractState
        {
            private GameBehaviourMachine _machine { get => _parentStateMachine as GameBehaviourMachine; }

            public override void OnEnter()
            {
                _machine._unitController.IsActive = true;
                _machine._unitController.UpdateTurn();
                _machine._uiManager.SetLayout(UIGameManager.Layout.Fight);
                _machine._uiManager.SetTimer(true);
            }
            public override void OnExit()
            {
                _machine._unitController.IsActive = false;
            }
        }
        public class GameOverState : AbstractState
        {
            private GameBehaviourMachine _machine { get => _parentStateMachine as GameBehaviourMachine; }

            public override void OnEnter()
            {
                _machine._uiManager.SetPlayerWin(_machine.PlayerWinID);
            }
        }
    }
}
