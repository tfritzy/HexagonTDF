using System.Collections.Generic;

public class Miner : Building
{
    public override LifeCell LifeCell => _lifeCell;
    public override Alliance Enemies => Alliance.Maltov;
    public override Alliance Alliance => Alliance.Player;
    public override BuildingType Type => BuildingType.Miner;
    public override ResourceCollectionCell ResourceCollectionCell => _resourceCollectionCell;
    public override ConveyorCell ConveyorCell => _conveyorCell;
    public override InventoryCell InventoryCell => _inventoryCell;
    public override string Name => charName;
    private const string charName = "Mine";
    public override Dictionary<ItemType, int> ItemsNeededForConstruction => _itemsNeededForConstruction;
    private Dictionary<ItemType, int> _itemsNeededForConstruction = new Dictionary<ItemType, int>
    {
        {ItemType.Log, 10},
        {ItemType.Plank, 20},
        {ItemType.StoneBlock, 30},
        {ItemType.SawBlade, 1},
        {ItemType.Shingle, 50},
    };

    private LifeCell _lifeCell;
    private ConveyorCell _conveyorCell;
    private ResourceCollectionCell _resourceCollectionCell;
    private InventoryCell _inventoryCell;
    public override void Setup()
    {
        _lifeCell = new MinerLifeCell();
        _resourceCollectionCell = new MinerResourceCollectionCell();
        _conveyorCell = new ConveyorCell(true);
        _inventoryCell = new InventoryCell(8);

        base.Setup();
    }
}