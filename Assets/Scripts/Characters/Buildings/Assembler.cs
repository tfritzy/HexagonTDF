public class Assembler : Building
{
    public override BuildingType Type => BuildingType.Assembler;
    public override LifeCell LifeCell => lifeCell;
    public override ResourceProcessingCell ResourceProcessingCell => assemblingCell;
    public override ConveyorCell ConveyorCell => conveyorCell;
    public override Alliance Enemies => Alliance.Maltov;
    public override Alliance Alliance => Alliance.Player;
    public override string Name => charName;
    private const string charName = "Assembler";

    private LifeCell lifeCell;
    private ConveyorCell conveyorCell;
    private ResourceProcessingCell assemblingCell;

    public override void Setup()
    {
        lifeCell = new AssemblerLifeCell();
        conveyorCell = new ConveyorCell(false);
        assemblingCell = new AssemblerProcessingCell();

        base.Setup();
    }
}