using WarForTheThrone.Map;
using UnityEngine;

namespace WarForTheThrone.Info
{
    [CreateAssetMenu(fileName = "UnitConfig", menuName = "Unit/Config")]
    public class UnitConfigSO : ScriptableObject
    {
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public int Health { get; set; }
        [field: SerializeField] public int Damage { get; set; }
        [field: SerializeField] public int AttackRange { get; set; }
        [field: SerializeField] public float HeightOffset { get; set; }
        [field: SerializeField] public Unit Prefab { get; set; }
        [field: SerializeField] public Sprite Preview { get; set; }
        [field: SerializeField] public Color InspirationWeaponColor { get; set; }
        [field: SerializeField] public UnitType Type { get; set; }
        public enum UnitType
        {
            Knight,
            Protector,
            Archer,
            Bannerman
        }
    }
}