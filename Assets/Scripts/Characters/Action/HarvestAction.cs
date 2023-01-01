public class HarvestAction : CharacterAction
{
    public HarvestAction(Character owner) : base(owner)
    {
    }

    public override void Start()
    {
        base.Start();

        this.Owner.ResourceCollectionCell.SetEnabled(true);
    }

    public override void End()
    {
        base.End();

        this.Owner.ResourceCollectionCell.SetEnabled(false);
    }
}