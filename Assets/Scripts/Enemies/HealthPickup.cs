using UnityEngine;
using CloneGame.Combat;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] private float healAmount = 25f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        Health health = other.GetComponent<Health>();

        if (health == null)
        {
            return;
        }

        health.Heal(healAmount);

        Destroy(gameObject);
    }
}
