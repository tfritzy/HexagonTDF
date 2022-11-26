public class LumberMillProcessingCell : ResourceProcessingCell
{
    public override ItemType OutputItemType => ItemType.Sticks;
    public override ItemType InputItemType => ItemType.Log;
    public override float SecondsToProcessResource => 3;
    public override InventoryCell InputInventory => inventoryCell;
    private InventoryCell inventoryCell;

    public override void Setup(Character owner)
    {
        inventoryCell = new InventoryCell(3);

        base.Setup(owner);
    }
}