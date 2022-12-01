public class Assembler : Building
{
    public override BuildingType Type => throw new System.NotImplementedException();
    public override LifeCell LifeCell => throw new System.NotImplementedException();
    public override Alliance Enemies => Alliance.Maltov;
    public override Alliance Alliance => Alliance.Player;
    public override string Name => charName;
    private const string charName = "Assembler";

    private LifeCell lifeCell;
    private ConveyorCell conveyorCell;
    
    protected override void Setup()
    {
        lifeCell = new AssemblerLifeCell();
        conveyorCell = new ConveyorCell(false);

        base.Setup();
    }
}