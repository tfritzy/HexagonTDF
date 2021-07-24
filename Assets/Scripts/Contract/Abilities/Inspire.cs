using System;

public class Inspire : InstantCastAbility
{
    public override float Cooldown => 60;
    private const float duration = 30f;
    private Guid id;

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
                building.AddEffect(new AttackSpeedEffect(.5f, duration, id, this.Owner));
            }
        }
    }
}