public class LumberMill : Building
{
    public override LifeCell LifeCell => lifeCell;
    public override Alliance Enemies => Alliance.Maltov;
    public override Alliance Alliance => Alliance.Player;
    public override BuildingType Type => BuildingType.LumberMill;
    public override ResourceProcessingCell ResourceProcessingCell => resourceProcessingCell;
    public override ConveyorCell ConveyorCell => conveyorCell;
    public override string Name => charName;
    private const string charName = "Lumber mill";

    private LifeCell lifeCell;
    private ConveyorCell conveyorCell;
    private ResourceProcessingCell resourceProcessingCell;
    public override void Setup()
    {
        lifeCell = new LumberMillLifeCell();
        resourceProcessingCell = new LumberMillProcessingCell();
        conveyorCell = new ConveyorCell(false);

        base.Setup();
    }
}