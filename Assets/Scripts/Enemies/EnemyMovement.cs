using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Enemy enemy;
    private Transform player;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        enemy = GetComponent<Enemy>();
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FixedUpdate()
    {
        if (player == null)
            return;

        Vector2 direction =
            (player.position - transform.position).normalized;

        rb.linearVelocity = direction * enemy.Data.MoveSpeed;
    }
}
