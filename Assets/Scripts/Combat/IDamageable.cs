namespace CloneGame.Combat
{
    public interface IDamageable
    {
        void TakeDamage(float amount, object source = null);
        bool IsAlive { get; }
    }
}