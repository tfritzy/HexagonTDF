public class Miner : Building
{
    public override LifeCell LifeCell => lifeCell;
    public override Alliance Enemies => Alliance.Maltov;
    public override Alliance Alliance => Alliance.Player;
    public override BuildingType Type => BuildingType.Miner;
    public override ResourceCollectionCell ResourceCollectionCell => resourceCollectionCell;
    public override ConveyorCell ConveyorCell => conveyorCell;
    public override string Name => charName;
    private const string charName = "Mine";

    private LifeCell lifeCell;
    private ConveyorCell conveyorCell;
    private ResourceCollectionCell resourceCollectionCell;
    public override void Setup()
    {
        lifeCell = new MinerLifeCell();
        resourceCollectionCell = new MinerResourceCollectionCell();
        conveyorCell = new ConveyorCell(true);

        base.Setup();
    }
}