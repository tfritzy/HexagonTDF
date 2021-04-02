using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    public abstract Alliances Alliance { get; }
    public abstract Alliances Enemies { get; }
    public abstract int StartingHealth { get; }
    public int Health
    {
        get { return health; }
        set
        {
            health = value;
            if (health <= 0)
            {
                Die();
            }
        }
    }
    private int health;

    void Start()
    {
        Setup();
    }

    protected virtual void Setup()
    {
        this.Health = StartingHealth;
    }

    void Update()
    {
        UpdateLoop();
    }
    protected virtual void UpdateLoop() { }

    protected virtual void Die()
    {
        Destroy(this.gameObject);
    }

    public void TakeDamage(int amount)
    {
        this.Health -= amount;
    }
}