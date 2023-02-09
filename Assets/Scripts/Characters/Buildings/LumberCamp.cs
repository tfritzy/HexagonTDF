using System.Collections.Generic;

public class LumberCamp : Building
{
    public override LifeCell LifeCell => _lifeCell;
    public override Alliance Enemies => Alliance.Maltov;
    public override Alliance Alliance => Alliance.Player;
    public override BuildingType Type => BuildingType.LumberCamp;
    public override ResourceCollectionCell ResourceCollectionCell => _resourceCollectionCell;
    public override ConveyorCell ConveyorCell => _conveyorCell;
    public override InventoryCell InventoryCell => _inventoryCell;
    public override string Name => charName;
    private const string charName = "Lumber camp";
    public override bool NeedsConstruction => false;
    public override Dictionary<ItemType, int> ItemsNeededForConstruction => _itemsNeededForConstruction;
    private Dictionary<ItemType, int> _itemsNeededForConstruction = new Dictionary<ItemType, int>
    {
        {ItemType.Log, 10},
        {ItemType.Rock, 30},
    };

    private LifeCell _lifeCell;
    private ConveyorCell _conveyorCell;
    private ResourceCollectionCell _resourceCollectionCell;
    private InventoryCell _inventoryCell;
    public override void Setup()
    {
        _lifeCell = new LumberCampLifeCell();
        _resourceCollectionCell = new LumberCampResourceCollectionCell();
        _conveyorCell = new ConveyorCell(true);
        _inventoryCell = new InventoryCell(8, "Lumber camp's inventory");

        base.Setup();
    }
}