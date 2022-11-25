public class LumberMill : Building
{
    public override BrainCell BrainCell => null;
    public override AttackCell AttackCell => null;
    public override LifeCell LifeCell => lifeCell;
    public override Alliance Enemies => Alliance.Maltov;
    public override Alliance Alliance => Alliance.Player;
    public override BuildingType Type => BuildingType.LumberMill;
    public override ResourceCollectionCell ResourceCollectionCell => null;
    public override ResourceProcessingCell ResourceProcessingCell => resourceProcessingCell;
    public override ConveyorCell ConveyorCell => conveyorCell;

    private LifeCell lifeCell;
    private ConveyorCell conveyorCell;
    private ResourceProcessingCell resourceProcessingCell;
    protected override void Setup()
    {
        lifeCell = new LumberMillLifeCell();
        resourceProcessingCell = new LumberMillProcessingCell();
        conveyorCell = new ConveyorCell(false);

        base.Setup();
    }
}