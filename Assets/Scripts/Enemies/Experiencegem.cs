using UnityEngine;

namespace CloneGame.Player
{
    /// <summary>
    /// Sits on an XP gem prefab. Enemies (or the Enemies & Difficulty system) just need
    /// to Instantiate this prefab on death — pickup and XP handling is automatic.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class ExperienceGem : MonoBehaviour
    {
        [SerializeField] private float xpValue = 1f;
        [SerializeField] private string playerTag = "Player";

        private void Reset()
        {
            var col = GetComponent<Collider2D>();
            if (col != null) col.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(playerTag)) return;

            if (other.TryGetComponent<PlayerExperience>(out var xp))
            {
                xp.AddExperience(xpValue);
                Destroy(gameObject);
            }
        }
    }
}