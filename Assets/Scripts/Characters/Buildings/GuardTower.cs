using System.Collections.Generic;

public class GuardTower : Building
{
    public override LifeCell LifeCell => _lifeCell;
    public override ItemPickupCell ItemPickupCell => _pickupCell;
    public override InventoryCell InventoryCell => _inventoryCell;
    public override ConveyorCell ConveyorCell => _conveyorCell;
    public override BrainCell BrainCell => _brainCell;
    public override AttackCell AttackCell => _attackCell;
    public override Alliance Enemies => Alliance.Maltov;
    public override Alliance Alliance => Alliance.Player;
    public override string Name => _name;
    public override BuildingType Type => BuildingType.GuardTower;
    public override Dictionary<ItemType, int> ItemsNeededForConstruction => _itemsNeededForConstruction;
    private Dictionary<ItemType, int> _itemsNeededForConstruction = new Dictionary<ItemType, int>
    {
        {ItemType.Log, 3},
        {ItemType.Rock, 10},
    };

    private string _name = "Guard Tower";
    private GuardTowerLifeCell _lifeCell;
    private ItemPickupCell _pickupCell;
    private InventoryCell _inventoryCell;
    private ConveyorCell _conveyorCell;
    private AttackTowerBrainCell _brainCell;
    private GuardTowerAttackCell _attackCell;

    public override void Setup()
    {
        List<ItemType> acceptedItems = new List<ItemType>(_itemsNeededForConstruction.Keys);
        acceptedItems.Add(ItemType.Arrow);

        _lifeCell = new GuardTowerLifeCell();
        _inventoryCell = new InventoryCell(8, "Guard tower's inventory");
        _conveyorCell = new ConveyorCell();
        _brainCell = new AttackTowerBrainCell();
        _attackCell = new GuardTowerAttackCell();
        _pickupCell = new ItemPickupCell(this.ConveyorCell, acceptedItems.ToArray(), this.InventoryCell);

        Arrow arrowStack = new Arrow();
        arrowStack.Quantity = arrowStack.MaxStackSize;
        _inventoryCell.AddItem(arrowStack);

        base.Setup();
    }
}