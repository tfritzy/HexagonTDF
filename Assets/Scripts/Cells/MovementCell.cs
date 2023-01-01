using UnityEngine;

public abstract class MovementCell : Cell
{
    public float MovementSpeed { get; private set; }
    public float MovementSpeedModification;
    protected abstract float BaseMovementSpeed { get; }

    public override void Setup(Character character)
    {
        base.Setup(character);
        this.MovementSpeed = BaseMovementSpeed;
    }

    public override void Update() { }
}