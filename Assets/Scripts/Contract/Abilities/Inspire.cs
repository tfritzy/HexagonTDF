using System;

public class Inspire : InstantCastAbility
{
    public override float Cooldown => 45f;
    private const float duration = 15f;
    private Guid id;
    private const float INCREASE_AMOUNT = .75f;

    public Inspire(Hero owner) : base(owner)
    {
        this.id = Guid.NewGuid();
    }

    protected override void Execute()
    {
        foreach (Building building in Managers.Board.Buildings.Values)
        {
            if (building is AttackTower && building.Alliance == this.Owner.Alliance)
            {
                building.AddEffect(new AttackSpeedEffect(INCREASE_AMOUNT, duration, id, this.Owner));
            }
        }
    }
}