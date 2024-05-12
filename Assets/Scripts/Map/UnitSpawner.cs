using WarForTheThrone.Info;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WarForTheThrone.Map
{
    public class UnitSpawner : MonoBehaviour
    {
        [SerializeField] private UnitController _controller;
        [SerializeField] private CellSpawner _cellSpawner;
        [SerializeField] private Color[] _playerColors;
        [SerializeField] private Transform[] _group;
        public Dictionary<Unit, Cell> Units { get; private set; } = new();

        public void CreateUnitAt(UnitConfigSO configSO, Cell cell, int owner, int priority)
        {
            Vector3 position = new Vector3(cell.transform.position.x, cell.transform.position.y + configSO.HeightOffset, cell.transform.position.z);
            Unit unit = Instantiate(configSO.Prefab, position, Quaternion.identity, _group[cell.Y]);
            unit.Initialize(owner, priority, configSO.HeightOffset, _controller, _playerColors[owner]);
            unit.Direction = owner == 0;
            Units.Add(unit, cell);
        }

        public Unit GetUnitByCell(Cell cell)
        {
            if(cell == null || !Units.ContainsValue(cell)) return null;
            return Units.First(element => element.Value == cell).Key;
        }
        public List<Unit> GetUnitsInCells(List<Cell> cells)
        {
            if (cells == null || cells.Count == 0) return null;
            List<Unit> units = new();
            foreach (var cell in cells)
            {
                var unit = GetUnitByCell(cell);
                if(unit!= null) 
                    units.Add(unit);
            }
            return units;
        }

        public int GetUnitsCountByOwner(int owner)
        {
            return Units.Count(unit => unit.Key.Owner == owner);
        }
        public Cell GetCellByUnit(Unit unit)
        {
            Cell cell;
            Units.TryGetValue(unit, out cell);
            return cell;
        }
        public void SwitchUnitToCell(Unit unit, Cell to)
        {
            Units[unit] = to;
            unit.transform.SetParent(_group[to.Y]);
        }
    }
}