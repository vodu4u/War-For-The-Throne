using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WarForTheThrone.Map
{
    public class CellSpawner : MonoBehaviour
    {
        [SerializeField] private Cell _prefab;
        [SerializeField] private Transform _world;
        [SerializeField] private Transform _startPoint;
        public List<Cell> Cells { get; private set; } = new();

        public void CreateGameField(int width, int height)
        {
            for (int i = 0; i < height; i++)
            {
                for (int i2 = 0; i2 < width; i2++)
                {
                    Vector3 spawn_pos;
                    spawn_pos = new Vector3(_startPoint.position.x + _prefab.Size * i2, _startPoint.position.y + _prefab.Size * i);

                    Cell cell = Instantiate(_prefab, spawn_pos, Quaternion.identity, _world);
                    cell.Initialize(i2, i);
                    Cells.Add(cell);
                }
            }
        }
    }
}