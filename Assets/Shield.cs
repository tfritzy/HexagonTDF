using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour, Damageable
{
    public Character Wielder;
    public const float DamageReductionAmount = .75f;

    public Alliances Alliance => Wielder.Alliance;

    public void TakeDamage(int amount, Character attacker)
    {
        Wielder.TakeDamage((int)(amount * DamageReductionAmount), attacker);
    }

    public bool IsNull()
    {
        return this == null || Wielder == null;
    }
}
