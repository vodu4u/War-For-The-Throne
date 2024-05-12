using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarForTheThrone.Map
{
    public class CellManager : MonoBehaviour
    {
        private Cell _lastHightlightedCell;
        [SerializeField] private Color _highlighted;
        [SerializeField] private Color _unit;
        [field: SerializeField]  public int MapWidth { get; set; }
        [field: SerializeField] public int MapHeight { get; set; }
        [field: SerializeField] public CellSpawner CellSpawner { get; set; }
        public int MapSize { get { return MapWidth * MapHeight; } }
        public enum ColorType 
        {
            Hightlight,
            Unit
        }
        private void Update()
        {
            Vector3 mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var cell = GetRayField(mouse_pos);
            if(cell != null && cell.IsAvaible)
            {
                HighlightCell(cell, ColorType.Hightlight);
            }
            else if(_lastHightlightedCell != null)
            {
                _lastHightlightedCell.ResetColor();
                _lastHightlightedCell = null;
            }
        }
        public Cell GetCell(int x, int y)
        {
            return CellSpawner.Cells.Find(cell => cell.X == x && cell.Y == y);
        }
        public void ResetCells(List<Cell> cells)
        {
            foreach(var cell in cells)
            {
                cell.HoldCustomColor = false;
                cell.ResetColor();
            }
        }
        public List<Cell> GetCells(Predicate<Cell> match)
        {
            return CellSpawner.Cells.FindAll(match);
        }
        public void SetCellsAvaible(List<Cell> cells, bool avaible)
        {
            foreach(var cell in cells)
            {
                cell.IsAvaible = avaible;
            }
        }
        public Cell GetRayField(Vector3 mouse_position)
        {
            RaycastHit2D hit = Physics2D.Raycast(mouse_position, Vector2.zero);
            return hit.transform?.GetComponent<Cell>();
        }
        public void HighlightCell(Cell cell, ColorType type)
        {
            if (_lastHightlightedCell != null && _lastHightlightedCell != cell)
                _lastHightlightedCell.ResetColor();

            switch (type)
            {
                case ColorType.Hightlight:
                    cell.SetCustomColor(_highlighted);
                    break;
                case ColorType.Unit:
                    cell.SetCustomColor(_unit);
                    cell.HoldCustomColor = true;
                    break;
            }

            _lastHightlightedCell = cell;
        }
        public List<Cell> GetCellsWithRangeFromCell(Cell cell, int range)
        {
            List<Cell> cells = new();
            for (int i = 0; i < MapSize; i++)
            {
                Cell field = CellSpawner.Cells[i];
                float distance = Mathf.FloorToInt(Mathf.Sqrt(Mathf.Pow(field.X - cell.X, 2) + Mathf.Pow(field.Y - cell.Y, 2)));
                if (distance <= range)
                {
                    cells.Add(field);
                }
            }
            return cells;
        }
    }
}