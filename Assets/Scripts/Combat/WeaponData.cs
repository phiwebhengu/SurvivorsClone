using UnityEngine;

namespace CloneGame.Combat
{
    [CreateAssetMenu(fileName = "NewWeapon", menuName = "CloneGame/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        public string weaponName = "Weapon";
        public float damage = 10f;
        public float cooldown = 1f;      // seconds between attacks
        public float range = 5f;         // max targeting distance
        public float projectileSpeed = 10f;
        public GameObject projectilePrefab;
    }
}