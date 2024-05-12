using WarForTheThrone.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarForTheThrone.Gameplay
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] private CellManager _cellManagement;

        private void Start()
        {
            _cellManagement.CellSpawner.CreateGameField(_cellManagement.MapWidth, _cellManagement.MapHeight);
        }
    }
}