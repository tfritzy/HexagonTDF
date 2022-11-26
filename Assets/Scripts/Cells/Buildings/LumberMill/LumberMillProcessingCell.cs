public class LumberMillProcessingCell : ResourceProcessingCell
{
    public override ResourceType OutputResourceType => ResourceType.Sticks;
    public override ResourceType InputResourceType => ResourceType.Log;
    public override float SecondsToProcessResource => 3;
    public override InventoryCell InputInventory => inventoryCell;
    private InventoryCell inventoryCell = new InventoryCell(1);
}