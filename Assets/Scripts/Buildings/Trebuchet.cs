using System.Collections.Generic;
using UnityEngine;

public class Trebuchet : Unit
{
    public override float BaseCooldown => 10;
    public override int BaseDamage => 100;
    public override float BaseRange => int.MaxValue;
    public override VerticalRegion AttackRegion => VerticalRegion.Ground;
    public override Alliances Alliance => Alliances.Player;
    public override Alliances Enemies => Alliances.Maltov;
    public override int StartingHealth => 25;
    public override float Power => int.MaxValue / 2;
    public override VerticalRegion Region => VerticalRegion.Ground;
    public GameObject ProtectionSpellAnimation;
    protected override float ProjectileSpeed => 20;
    public override bool IsMelee => false;

    protected override void Setup()
    {
        base.Setup();
        RecalculatePath();

        ProtectionSpellAnimation = Instantiate(ProtectionSpellAnimation, this.transform);
        Helpers.TriggerAllParticleSystems(ProtectionSpellAnimation.transform, false);
        ProtectionSpellAnimation.SetActive(false);
        SetHealthbarValues();
    }

    protected override bool IsInRangeOfTarget()
    {
        return true;
    }

    protected override Character FindTargetCharacter()
    {
        foreach (Building building in Managers.Board.Buildings.Values)
        {
            if (building.Alliance == this.Enemies)
            {
                return building;
            }
        }

        return null;
    }

    public override void TakeDamage(int amount, Character source)
    {
        base.TakeDamage(amount, source);
        ProtectTrebuchetSpell(source);
        SetHealthbarValues();
    }

    private void SetHealthbarValues()
    {
        Managers.TrebuchetHealthbar.SetValue(this.Health, this.StartingHealth);
    }

    private void ProtectTrebuchetSpell(Character attacker)
    {
        attacker.TakeDamage(int.MaxValue / 2, this);
        ProtectionSpellAnimation.transform.position = attacker.transform.position + Vector3.up * .01f;
        ProtectionSpellAnimation.SetActive(true);
        Helpers.TriggerAllParticleSystems(ProtectionSpellAnimation.transform, true);
    }

    protected override bool IsCollisionTarget(Character attacker, GameObject other)
    {
        if (other.TryGetComponent<Building>(out Building building))
        {
            if (building.Alliance == this.Enemies)
            {
                return true;
            }
        }

        return false;
    }

    protected override void CalculateNextPathingPosition(Vector2Int currentPosition)
    {
        this.Waypoint = new Waypoint(this.GridPosition, this.GridPosition, Vector3.zero);
    }

    protected override void RecalculatePath()
    {
        CalculateNextPathingPosition(this.GridPosition);
    }

    protected override bool ShouldRecalculatePath()
    {
        return false;
    }
}