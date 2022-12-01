public class StoneCarver : Building
{
    public override LifeCell LifeCell => lifeCell;
    public override Alliance Enemies => Alliance.Maltov;
    public override Alliance Alliance => Alliance.Player;
    public override BuildingType Type => BuildingType.StoneCarver;
    public override ConveyorCell ConveyorCell => conveyorCell;
    public override ResourceProcessingCell ResourceProcessingCell => resourceProcessingCell;
    public override string Name => charName;
    private const string charName = "Stone carver";

    private LifeCell lifeCell;
    private ConveyorCell conveyorCell;
    private ResourceProcessingCell resourceProcessingCell;
    protected override void Setup()
    {
        lifeCell = new StoneCarverLifeCell();
        resourceProcessingCell = new StoneCarverResourceProcessingCell();
        conveyorCell = new ConveyorCell(false);

        base.Setup();
    }
}