using UnityEngine;

public class OpenInventoryAction : CharacterAction
{
    public override MainCharacterAnimationState Animation => MainCharacterAnimationState.Idle;
    private Character target;

    public OpenInventoryAction(Character owner, Character target) : base(owner)
    {
        this.target = target;
    }

    public override void Start()
    {
        base.Start();

        Vector3 distToTarget = target.transform.position - Owner.transform.position;
        if (distToTarget.sqrMagnitude > 1)
        {
            this.End();
        }
        else
        {
            Managers.UI.ShowPage(Page.CharacterSelectionModal);
        }
    }

    public override void Update()
    {
        base.Update();

        CharacterSelectionModal modal = (CharacterSelectionModal)Managers.UI.GetPage(Page.CharacterSelectionModal);
        modal.Update(this.Owner, target);
    }

    public override void End()
    {
        base.End();

        this.Owner.ResourceCollectionCell.Reset();
        this.Owner.ResourceCollectionCell.SetEnabled(false);
    }
}