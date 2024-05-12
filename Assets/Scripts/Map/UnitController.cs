using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WarForTheThrone.Extension;
using WarForTheThrone.UI;
using System;
using WarForTheThrone.Audio;
using WarForTheThrone.Gameplay;

namespace WarForTheThrone.Map
{
    public class UnitController : MonoBehaviour
    {
        private readonly int[] _playersPriority = new int[2];
        [SerializeField] private GameBehaviourMachine _stateMachine;
        [SerializeField] private UnitSpawner _unitSpawner;
        [SerializeField] private UnitPlacer _unitPlacer;
        [SerializeField] private AudioManager _audioManager;
        [SerializeField] private CellManager _cellManagement;
        [SerializeField] private UIGameManager _uiManagement;
        [SerializeField] private Arrow _arrow;
        private Unit _currentUnit;
        private int _turn;
        public bool IsActive { get; set; }

        public void UpdateTurn()
        {
            List<Unit> unitsOfPlayer = _unitSpawner.Units.Keys.ToList().FindAll(unit => unit.Owner == _turn);
            if (unitsOfPlayer.Count == 0)
            {
                _stateMachine.PlayerWinID = _turn == 0 ? 1 : 0;
                _stateMachine.TransitionToState(GameBehaviourMachine.GameplayState.GAME_OVER);
            }
            else
            {
                _uiManagement.ChangeTurn(_turn);
                List<Unit> unitsWithPriority = unitsOfPlayer.FindAll(unit => unit.Priority >= _playersPriority[_turn]);
                if (unitsWithPriority.Count == 0)
                {
                    _playersPriority[_turn] = 0;
                    _currentUnit = unitsOfPlayer.First(unity => unity.Priority >= 0);
                }
                else
                {
                    _currentUnit = unitsWithPriority[0];
                }
                Cell cell = _unitSpawner.GetCellByUnit(_currentUnit);

                _cellManagement.HighlightCell(cell, CellManager.ColorType.Unit);
                List<Cell> avaibleCells = new();
                if (_currentUnit.Config.Type == Info.UnitConfigSO.UnitType.Knight)
                {
                    avaibleCells = _cellManagement.GetCellsWithRangeFromCell(cell, 1);
                    Extensions.AddIfNotNull(avaibleCells, _cellManagement.GetCell(cell.X, cell.Y + 2));
                    Extensions.AddIfNotNull(avaibleCells, _cellManagement.GetCell(cell.X, cell.Y - 2));
                    Extensions.AddIfNotNull(avaibleCells, _cellManagement.GetCell(cell.X + 2, cell.Y));
                    Extensions.AddIfNotNull(avaibleCells, _cellManagement.GetCell(cell.X - 2, cell.Y));
                }
                else if (_currentUnit.Config.Type == Info.UnitConfigSO.UnitType.Protector)
                {
                    avaibleCells = _cellManagement.GetCellsWithRangeFromCell(cell, 1);
                    for (int i = 2; i < 3; i++)
                    {
                        Extensions.AddIfNotNull(avaibleCells, _cellManagement.GetCell(cell.X + i, cell.Y + i));
                        Extensions.AddIfNotNull(avaibleCells, _cellManagement.GetCell(cell.X - i, cell.Y + i));
                        Extensions.AddIfNotNull(avaibleCells, _cellManagement.GetCell(cell.X + i, cell.Y - i));
                        Extensions.AddIfNotNull(avaibleCells, _cellManagement.GetCell(cell.X - i, cell.Y - i));
                    }
                }
                else if (_currentUnit.Config.Type == Info.UnitConfigSO.UnitType.Archer)
                {
                    Extensions.AddIfNotNull(avaibleCells, _cellManagement.GetCell(cell.X - 1, cell.Y));
                    Extensions.AddIfNotNull(avaibleCells, _cellManagement.GetCell(cell.X + 1, cell.Y));
                    for (int i = 0; i < 2; i++)
                    {
                        Extensions.AddIfNotNull(avaibleCells, _cellManagement.GetCell(cell.X, cell.Y + i));
                        Extensions.AddIfNotNull(avaibleCells, _cellManagement.GetCell(cell.X, cell.Y - i));
                    }
                }
                else if(_currentUnit.Config.Type == Info.UnitConfigSO.UnitType.Bannerman)
                {
                    avaibleCells = _cellManagement.GetCellsWithRangeFromCell(cell, 2);
                    Extensions.RemoveIfNotNull(avaibleCells, _cellManagement.GetCell(cell.X + 2, cell.Y + 2));
                    Extensions.RemoveIfNotNull(avaibleCells, _cellManagement.GetCell(cell.X - 2, cell.Y + 2));
                    Extensions.RemoveIfNotNull(avaibleCells, _cellManagement.GetCell(cell.X + 2, cell.Y - 2));
                    Extensions.RemoveIfNotNull(avaibleCells, _cellManagement.GetCell(cell.X - 2, cell.Y - 2));
                }
                _cellManagement.SetCellsAvaible(_cellManagement.CellSpawner.Cells, false);
                _cellManagement.SetCellsAvaible(_cellManagement.GetCells(cell => !_unitSpawner.Units.ContainsValue(cell) && avaibleCells.Contains(cell)), true);
                IsActive = true;
            }
        }
        public void ChangeTurn()
        {
            _currentUnit.Inspiration = false;
            _playersPriority[_turn] = _currentUnit.Priority + 1;
            _turn = _turn == 0 ? 1 : 0;
            UpdateTurn();
        }
        public void SkipTurn()
        {
            _cellManagement.ResetCells(_cellManagement.CellSpawner.Cells);
            AttackIfNearEnemyUnit();
        }
        private void Update()
        {
            if (IsActive && Input.GetMouseButtonDown(0))
            {
                Vector3 mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Cell cell = _cellManagement.GetRayField(mouse_pos);

                if (cell != null && cell.IsAvaible)
                {
                    MoveUnit(cell);
                }
            }

        }
        private void MoveUnit(Cell cell)
        {
            _currentUnit.FacePosition(cell.transform.position);
            _cellManagement.ResetCells(_cellManagement.CellSpawner.Cells);
            _cellManagement.SetCellsAvaible(_cellManagement.CellSpawner.Cells, false);
            _unitSpawner.SwitchUnitToCell(_currentUnit, cell);
            _currentUnit.MoveToCell(cell, () => AttackIfNearEnemyUnit());
            IsActive = false;
        }

        private void AttackIfNearEnemyUnit()
        {
            bool wasAttack = false;
            Cell currentUnitCell = _unitSpawner.GetCellByUnit(_currentUnit);
            if (_currentUnit.Config.Type == Info.UnitConfigSO.UnitType.Knight || _currentUnit.Config.Type == Info.UnitConfigSO.UnitType.Protector ||
                _currentUnit.Config.Type == Info.UnitConfigSO.UnitType.Bannerman)
            {
                // Melee attack
                wasAttack = AttackIfHasEnemyInCell(_cellManagement.GetCell(currentUnitCell.X + (_currentUnit.Direction ? 1 : -1), currentUnitCell.Y));
                if (!wasAttack) wasAttack = AttackIfHasEnemyInCell(_cellManagement.GetCell(currentUnitCell.X + (_currentUnit.Direction ? -1 : 1), currentUnitCell.Y));
                if (!wasAttack && _currentUnit.Config.Type == Info.UnitConfigSO.UnitType.Bannerman)
                {
                    _currentUnit.PlayAnimationOnce(Unit.AnimationClip.inspiration);
                    _audioManager.PlaySound(AudioManager.Sounds.Inspiration);
                    List<Cell> cellNearUnit = _cellManagement.GetCellsWithRangeFromCell(currentUnitCell, 1);
                    List<Unit> units = _unitSpawner.GetUnitsInCells(cellNearUnit);
                    units = units.FindAll(unit => unit.Owner == _turn);
                    foreach (var unit in units)
                    {
                        unit.Inspiration = true;
                    }
                }
                ChangeTurn();
            }
            else if (_currentUnit.Config.Type == Info.UnitConfigSO.UnitType.Archer)
            {
                Cell cellWithEnemy = null;
                Unit enemy = null;
                for (int i = 1; i <= _currentUnit.Config.AttackRange; i++)
                {
                    var cell = _cellManagement.GetCell(_currentUnit.Direction ? currentUnitCell.X + i : currentUnitCell.X - i, currentUnitCell.Y);
                    if (_unitSpawner.Units.ContainsValue(cell))
                    {
                        var unit = _unitSpawner.GetUnitByCell(cell);
                        if(unit.Owner != _turn)
                        {
                            cellWithEnemy = cell;
                            enemy = _unitSpawner.GetUnitByCell(cellWithEnemy);
                            break;
                        }
                    }
                }
                if (enemy != null) 
                {
                    _currentUnit.PlayAnimationOnce(Unit.AnimationClip.attack);
                    var coroutine = AnimateArrowAfterDelay(enemy, ()=> ChangeTurn());
                    StartCoroutine(coroutine);
                }
                else
                {
                    ChangeTurn();
                }
            }
        }
        private bool AttackIfHasEnemyInCell(Cell cell)
        {       
            if (cell == null) return false;
            Unit enemy = _unitSpawner.GetUnitByCell(cell);
            if (enemy != null && enemy.Owner != _turn)
            {
                if (_currentUnit.Config.Type == Info.UnitConfigSO.UnitType.Archer) return true;
                AttackUnitByCurrentUnit(enemy);
                return true;
            }
            return false;
        }
        private void AttackUnitByCurrentUnit(Unit unitToAttack)
        {
            _audioManager.PlaySound(AudioManager.Sounds.Attack);
            _currentUnit.FacePosition(unitToAttack.transform.position);
            _currentUnit.PlayAnimationOnce(Unit.AnimationClip.attack);
            DamageUnit(unitToAttack, _currentUnit.GetDamage(), Unit.DamageSource.melee);
        }
        public void UnitDied(Unit unit)
        {
            _unitSpawner.Units.Remove(unit);
            _uiManagement.UpdateUnitsAlive(unit.Owner, _unitSpawner.GetUnitsCountByOwner(unit.Owner), _unitPlacer.MaxUnitsForPlayer);
        }
        private IEnumerator AnimateArrowAfterDelay(Unit enemy, Action onEnd)
        {
            yield return new WaitForSeconds(.7f);
            _audioManager.PlaySound(AudioManager.Sounds.Arrow);
            _arrow.gameObject.SetActive(true);
            _arrow.transform.position = new Vector2(_currentUnit.transform.position.x, _currentUnit.transform.position.y + 5);
            _arrow.Animate(new Vector2(enemy.transform.position.x, enemy.transform.position.y + 5), _currentUnit.Direction, .5f, delegate
            {
                DamageUnit(enemy, _currentUnit.GetDamage(),  Unit.DamageSource.arrow);
                onEnd?.Invoke();
            });
        }
        private void DamageUnit(Unit unit, int damage, Unit.DamageSource source) 
        {
            int newDamage = unit.Damage(damage, source);
            if (newDamage == damage)
            {
                _audioManager.PlaySound(AudioManager.Sounds.Damaged);
            }
            else
            {
                _audioManager.PlaySound(AudioManager.Sounds.Block);
            }
            _uiManagement.AddScore(newDamage, _turn);
        }
    }
}