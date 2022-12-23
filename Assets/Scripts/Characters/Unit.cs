using UnityEngine;

public abstract class Unit : Character
{
    private Rigidbody rb;
    private UnitAnimationState AnimationState;
    private Animator Animator;
    private const string AnimationStateName = "AnimationState";
    private HealthBar healthbar;

    public void SetAnimationState(UnitAnimationState newState)
    {
        this.AnimationState = newState;
        this.Animator.SetInteger(AnimationStateName, (int)newState);
    }

    public Rigidbody Rigidbody
    {
        get
        {
            if (rb == null)
            {
                rb = this.GetComponent<Rigidbody>();
            }
            return rb;
        }
    }

    public override void Setup()
    {
        base.Setup();
        if (this.Rigidbody != null)
        {
            this.Rigidbody.constraints =
                RigidbodyConstraints.FreezePositionY |
                RigidbodyConstraints.FreezeRotationX |
                RigidbodyConstraints.FreezeRotationZ;
        }

        this.Animator = this.Body.GetComponent<Animator>();
    }
}