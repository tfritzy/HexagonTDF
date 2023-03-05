using System.Collections.Generic;
using UnityEngine;

public class MainCharacterBrainCell : BrainCell
{
    private LinkedList<CharacterAction> CurrentActions = new LinkedList<CharacterAction>();
    private MainCharacterResourceCollectionCell harvestCell => (MainCharacterResourceCollectionCell)this.Owner.ResourceCollectionCell;
    private Rigidbody rb;

    public MainCharacterBrainCell()
    {
    }

    public override void Update()
    {
        if (this.rb == null)
        {
            this.rb = this.Owner.GetComponent<Rigidbody>();
        }

        Vector3 inputDir = GetDirectionalInput();
        if (inputDir != Vector3.zero)
        {
            SetVelocity(inputDir * this.Owner.MovementCell.MovementSpeed);
            this.Owner.transform.forward = inputDir;
            this.Owner.Animator?.SetInteger(
                Constants.AnimationStateParameter,
                (int)MainCharacterAnimationState.Running
            );
        }
        else
        {
            SetVelocity(Vector3.zero);
            this.Owner.Animator?.SetInteger(
                Constants.AnimationStateParameter,
                (int)MainCharacterAnimationState.Idle
            );
        }
    }

    private void SetVelocity(Vector3 target)
    {
        target.y = this.rb.velocity.y;
        this.rb.velocity = target;
    }

    public Vector3 GetDirectionalInput()
    {
        Vector3 direction = Vector3.zero;

        if (Input.GetKey(KeyCode.D))
        {
            direction += Vector3.right;
        }

        if (Input.GetKey(KeyCode.W))
        {
            direction += Vector3.forward;
        }

        if (Input.GetKey(KeyCode.S))
        {
            direction += Vector3.back;
        }

        if (Input.GetKey(KeyCode.A))
        {
            direction += Vector3.left;
        }

        return direction.normalized;
    }
}