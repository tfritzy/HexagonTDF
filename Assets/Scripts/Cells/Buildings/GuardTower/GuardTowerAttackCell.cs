public class GuardTowerAttackCell : AttackCell
{
    public override float BaseCooldown => 3f;
    public override int BaseDamage => 25;
    public override float BaseRange => 5f;
    public override VerticalRegion AttackRegion => VerticalRegion.GroundAndAir;
    public override VerticalRegion Region => VerticalRegion.Ground;
    public override bool InstantaneousAttacks => false;
    public override ItemType Ammo => ItemType.Arrow;
}