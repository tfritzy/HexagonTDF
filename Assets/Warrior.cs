using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : Hero
{
    public override Alliances Alliance => Alliances.Player;
    public override Alliances Enemies => Alliances.Illigons;
    public override int StartingHealth => 15;
    public override float Power => 100;
    public override VerticalRegion Region => VerticalRegion.Ground;
    public override float BaseCooldown => AttackSpeed.Medium;
    public override int BaseDamage => 3;
    public override int BaseRange => 1;
    public override VerticalRegion AttackRegion => VerticalRegion.Ground;
    protected override AnimationState AttackAnimation => AnimationState.SlashingSword;
    protected override float BaseMovementSpeed => 3f;
    public override bool IsMelee => true;

    protected override void InitializeAbilities()
    {
        this.Abilities = new List<Ability>()
        {
            new Consecrate(this),
            new Inspire(this),
        };
    }
}
