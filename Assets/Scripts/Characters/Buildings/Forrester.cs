public class Forrester : Building
{
    public override BrainCell BrainCell => null;
    public override AttackCell AttackCell => null;
    public override LifeCell LifeCell => lifeCell;
    public override Alliance Enemies => Alliance.Maltov;
    public override Alliance Alliance => Alliance.Player;
    public override BuildingType Type => BuildingType.Forrester;
    public override ResourceCollectionCell ResourceCollectionCell => resourceCollectionCell;
    public override ConveyorCell ConveyorCell => conveyorCell;

    private LifeCell lifeCell;
    private ConveyorCell conveyorCell;
    private ResourceCollectionCell resourceCollectionCell;
    protected override void Setup()
    {
        lifeCell = new ForresterLifeCell();
        resourceCollectionCell = new ForresterResourceCollectionCell();
        conveyorCell = new ConveyorCell(true);

        base.Setup();
    }
}