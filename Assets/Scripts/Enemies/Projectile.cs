using CloneGame.Combat;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private float lifeTime = 5f;

    private Vector2 direction;
    private float damage;

    public void Initialize(Vector2 shootDirection, float projectileDamage)
    {
        direction = shootDirection.normalized;
        damage = projectileDamage;

        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.position +=
            (Vector3)(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        IDamageable damageable = other.GetComponent<IDamageable>();

        if (damageable == null)
        {
            return;
        }
      
        damageable.TakeDamage(damage, gameObject);

        Destroy(gameObject);
    }
}
