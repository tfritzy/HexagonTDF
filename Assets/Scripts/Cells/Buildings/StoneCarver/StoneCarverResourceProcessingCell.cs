public class StoneCarverResourceProcessingCell : ResourceProcessingCell
{
    public override ItemType OutputItemType => ItemType.ArrowHead;
    public override ItemType InputItemType => ItemType.Rock;
    public override float SecondsToProcessResource => 1;
    public override InventoryCell InputInventory => inventoryCell;
    public override float PercentOfInputConsumedPerOutput => 1f;

    private InventoryCell inventoryCell;

    public override void Setup(Character owner)
    {
        inventoryCell = new InventoryCell(3, "Inventory");

        base.Setup(owner);
    }
}