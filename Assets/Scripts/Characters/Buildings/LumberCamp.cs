using System.Collections.Generic;

public class LumberCamp : Building
{
    public override LifeCell LifeCell => lifeCell;
    public override Alliance Enemies => Alliance.Maltov;
    public override Alliance Alliance => Alliance.Player;
    public override BuildingType Type => BuildingType.LumberCamp;
    public override ResourceCollectionCell ResourceCollectionCell => resourceCollectionCell;
    public override ConveyorCell ConveyorCell => conveyorCell;
    public override string Name => charName;
    private const string charName = "Lumber camp";
    public override Dictionary<ItemType, int> ItemsNeededForConstruction => _itemsNeededForConstruction;
    private Dictionary<ItemType, int> _itemsNeededForConstruction = new Dictionary<ItemType, int>
    {
        {ItemType.Log, 10},
        {ItemType.Plank, 20},
        {ItemType.StoneBlock, 30},
        {ItemType.SawBlade, 1},
        {ItemType.Shingle, 50},
    };

    private LifeCell lifeCell;
    private ConveyorCell conveyorCell;
    private ResourceCollectionCell resourceCollectionCell;
    public override void Setup()
    {
        lifeCell = new LumberCampLifeCell();
        resourceCollectionCell = new LumberCampResourceCollectionCell();
        conveyorCell = new ConveyorCell(true);

        base.Setup();
    }
}