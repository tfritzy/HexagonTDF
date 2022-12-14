public class GuardTower : Building
{
    public override LifeCell LifeCell => _lifeCell;
    public override ItemPickupCell ItemPickupCell => _pickupCell;
    public override InventoryCell InventoryCell => _inventoryCell;
    public override Alliance Enemies => Alliance.Maltov;
    public override Alliance Alliance => Alliance.Player;
    public override string Name => _name;
    public override BuildingType Type => BuildingType.GuardTower;

    private string _name = "Guard Tower";
    private GuardTowerLifeCell _lifeCell;
    private ItemPickupCell _pickupCell;
    private InventoryCell _inventoryCell;

    protected override void Setup()
    {
        _lifeCell = new GuardTowerLifeCell();
        _pickupCell = new ItemPickupCell(null, this.InventoryCell);
        _inventoryCell = new InventoryCell(8);

        base.Setup();
    }
}