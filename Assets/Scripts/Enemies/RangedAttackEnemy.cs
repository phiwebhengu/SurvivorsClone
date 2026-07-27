using CloneGame.Player;
using UnityEngine;

public class RangedAttackEnemy : MonoBehaviour
{
    [Header("Projectile")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform firePoint;

    [Header("Combat")]
    [SerializeField] private float attackRange = 6f;
    [SerializeField] private float attackCooldown = 2f;

    private Enemy enemy;
    private EnemyMovement movement;
    private Transform player;

    private float cooldownTimer;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        movement = GetComponent<EnemyMovement>();
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (player == null)
            return;

        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > attackRange)
        {
            movement.CanMove = true;
            return;
        }

        movement.CanMove = false;

        if (cooldownTimer > 0f)
        {
            return;
        }

        Shoot();

        cooldownTimer = attackCooldown;
    }

    private void Shoot()
    {
        Vector2 direction =
            (player.position - firePoint.position).normalized;

        Projectile projectile =
            Instantiate(projectilePrefab,
                        firePoint.position,
                        Quaternion.identity);

        projectile.Initialize(direction, enemy.Data.Damage);
    }
}