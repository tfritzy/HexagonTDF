using UnityEngine;

public abstract class MovementCell : Cell
{
    public abstract Vector3 Velocity { get; }
    protected abstract float BaseMovementSpeed { get; }

    public float MovementSpeedModification;
    public float MovementSpeed { get; private set; }

    public override void Setup(Character character)
    {
        base.Setup(character);
        this.MovementSpeed = BaseMovementSpeed;
    }
}