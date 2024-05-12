using UnityEngine;

namespace WarForTheThrone.Info
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Game/Config")]
    public class GameConfigSO : ScriptableObject
    {
        [field: SerializeField] public UnitConfigSO[] Units { get; set; }
    }
}