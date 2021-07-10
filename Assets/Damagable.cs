interface Damageable
{
    void TakeDamage(int amount, Character attacker);
    Alliances Alliance { get; }
    bool IsNull();
}