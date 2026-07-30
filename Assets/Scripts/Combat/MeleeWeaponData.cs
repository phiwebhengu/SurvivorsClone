using UnityEngine;

namespace CloneGame.Combat
{
    [CreateAssetMenu(fileName = "NewMeleeWeapon", menuName = "CloneGame/Melee Weapon Data")]
    public class MeleeWeaponData : ScriptableObject
    {
        public string weaponName = "Melee Weapon";
        public float damage = 8f;
        public float cooldown = 0.6f;
        public float range = 1.8f; // how far in front of the player the swing reaches
        public float width = 1.2f; // how wide the swing arc is
    }
}
