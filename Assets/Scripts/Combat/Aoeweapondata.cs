using UnityEngine;
 
namespace CloneGame.Combat
{
    [CreateAssetMenu(fileName = "NewAoEWeapon", menuName = "CloneGame/AoE Weapon Data")]
    public class AoEWeaponData : ScriptableObject
    {
        public string weaponName = "AoE Weapon";
        public float damage = 5f;
        public float cooldown = 1f;   // seconds between pulses
        public float radius = 2.5f;   // damage radius around the player
    }
}
 
