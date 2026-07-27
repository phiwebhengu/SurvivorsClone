using CloneGame.Combat;
using UnityEngine;

public class MeleeAttackEnemy : MonoBehaviour
{
    [SerializeField] private float attackCooldown = 0.2f;

    private Enemy enemy;
    private float cooldownTimer;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    private void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log("Colliding with: " + collision.gameObject.name);

        if (cooldownTimer > 0f)
            return;

        if (!collision.gameObject.CompareTag("Player"))
            return;

        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();

        if (damageable == null)
            return;

        damageable.TakeDamage(enemy.Data.Damage, gameObject);

        cooldownTimer = attackCooldown;
    }
}