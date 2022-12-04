public class LumberMillProcessingCell : ResourceProcessingCell
{
    public override ItemType OutputItemType => ItemType.ArrowShaft;
    public override float SecondsToProcessResource => 1;
    public override InventoryCell InputInventory => inventoryCell;
    public override float PercentOfInputConsumedPerOutput => .2f;

    private InventoryCell inventoryCell;

    public override void Setup(Character owner)
    {
        inventoryCell = new InventoryCell(3, "Inventory");

        base.Setup(owner);
    }
}