public class Conveyer : Building
{
    public override BuildingType Type => BuildingType.Conveyor;
    public override BrainCell BrainCell => null;
    public override AttackCell AttackCell => null;
    public override LifeCell LifeCell => lifeCell;
    public override ResourceCollectionCell ResourceCollectionCell => null;
    public override Alliance Enemies => Alliance.Maltov;
    public override Alliance Alliance => Alliance.Player;

    private LifeCell lifeCell;
    protected override void Setup()
    {
        lifeCell = new ConveyerLifeCell();

        base.Setup();
    }
}