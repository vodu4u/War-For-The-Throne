using WarForTheThrone.Info;
using WarForTheThrone.UI;
using System;
using UnityEngine;

namespace WarForTheThrone.Map
{
    public class UnitPlacer : MonoBehaviour
    {
        [SerializeField] private int _placerRangeForPlayer = 5;
        [SerializeField] private CellManager _cellManagement;
        [SerializeField] private UnitSpawner _unitSpawner;
        [SerializeField] private UIGameManager _uiManager;
        private int _ownerId;
        private int _priority;
        private Action _onEndPlacement;
        [field: SerializeField] public int MaxUnitsForPlayer { get; set; } = 5;
        public UnitConfigSO Unit { get; set; }
        public bool IsActive { get; set; }
        public void Initialize(Action onEndPlacement)
        {
            _ownerId = 0;
            _priority = 0;
            _onEndPlacement = onEndPlacement;
            SetActive(_ownerId, _priority);
            _uiManager.UpdateUnitsAlive(0, 0, MaxUnitsForPlayer);
            _uiManager.UpdateUnitsAlive(1, 0, MaxUnitsForPlayer);
        }

        void Update()
        {
            if (IsActive && Input.GetMouseButtonDown(0))
            {
                Vector3 mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Cell cell = _cellManagement.GetRayField(mouse_pos);
                if(cell != null && cell.IsAvaible)
                {
                    _unitSpawner.CreateUnitAt(Unit, cell, _ownerId, _priority);
                    _uiManager.UpdateUnitsAlive(_ownerId, _unitSpawner.GetUnitsCountByOwner(_ownerId), MaxUnitsForPlayer);
                    if (_ownerId == 1) _priority++;
                    SetActive(SwitchOwner(), _priority);
                }
            }

        }
        private void SetActive(int ownerID, int priority)
        {
            _ownerId = ownerID;
            _priority = priority;
            if(ownerID == 0)
            {
                _cellManagement.SetCellsAvaible(_cellManagement.GetCells(cell => cell.X < _placerRangeForPlayer && !_unitSpawner.Units.ContainsValue(cell)), true);
                _cellManagement.SetCellsAvaible(_cellManagement.GetCells(cell => cell.X > _placerRangeForPlayer || _unitSpawner.Units.ContainsValue(cell)), false);
            }
            else
            {
                _cellManagement.SetCellsAvaible(_cellManagement.GetCells(cell => cell.X >= _cellManagement.MapWidth - _placerRangeForPlayer && !_unitSpawner.Units.ContainsValue(cell)), true);
                _cellManagement.SetCellsAvaible(_cellManagement.GetCells(cell => cell.X < _cellManagement.MapWidth - _placerRangeForPlayer || _unitSpawner.Units.ContainsValue(cell)), false);
            }
            if (PlacementEnded())
            {
                _onEndPlacement?.Invoke();
            }
        }
        private int SwitchOwner()
        {
            return _ownerId == 0 ? 1 : 0;
        }
        private bool PlacementEnded()
        {
            return _priority == MaxUnitsForPlayer;
        }
    }
}
