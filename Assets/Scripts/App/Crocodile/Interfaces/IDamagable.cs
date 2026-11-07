namespace App.Crocodile.Interfaces
{
    public interface IDamagable
    {
        bool IsBombDebufActive { get; set; }
        void ApplyDamage(float damage, bool needShake = true, bool closeDamage = true);
    }
}