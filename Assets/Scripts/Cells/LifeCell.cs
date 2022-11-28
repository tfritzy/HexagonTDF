using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class LifeCell : Cell
{
    public abstract int StartingHealth { get; }
    public bool IsDead { get; private set; }
    private int health;
    private Healthbar healthbar;
    private List<TakeDamageTiming> damageTimings;

    internal class TakeDamageTiming
    {
        public float DamageTime;
        public int Damage;
        public Character Source;
    }

    public override void Update()
    {
        ProcessTakeDamageTimings();
    }

    public override void Setup(Character character)
    {
        base.Setup(character);

        if (StartingHealth == 0)
        {
            throw new System.Exception("Starting health should not be 0.");
        }

        this.Health = StartingHealth;

        // if (this.healthbar == null)
        // {
        //     this.healthbar = GameObject.Instantiate(Prefabs.Healthbar,
        //                 new Vector3(10000, 10000),
        //                 new Quaternion(),
        //                 Managers.Canvas).GetComponent<Healthbar>();
        //     this.healthbar.SetOwner(this.Owner.Body);
        //     this.healthbar.enabled = false;
        // }

        damageTimings = new List<TakeDamageTiming>();
    }


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

    protected virtual void Die()
    {
        this.Owner.DisableAllCells();
        this.IsDead = true;
        GameObject.Destroy(this.Owner.gameObject);
    }

    private void ProcessTakeDamageTimings()
    {
        while (this.damageTimings.Count > 0 && Time.time > this.damageTimings.Last().DamageTime)
        {
            this.TakeDamage(this.damageTimings.Last().Damage, this.damageTimings.Last().Source);
            this.damageTimings.RemoveAt(this.damageTimings.Count - 1);
        }
    }

    public void TakeDamage(int amount, Character source)
    {
        this.Health -= amount;
        this.healthbar.enabled = true;
        this.healthbar.SetFillScale((float)this.Health / (float)this.StartingHealth);
    }

    public void TakeDamage(int amount, Character source, float delay)
    {
        this.damageTimings.Add(new TakeDamageTiming
        {
            Damage = amount,
            DamageTime = Time.time + delay,
            Source = source,
        });

        this.damageTimings.Sort((x, y) => y.DamageTime.CompareTo(x.DamageTime));
    }
}