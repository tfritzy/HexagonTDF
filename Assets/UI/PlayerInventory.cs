using System.Collections.Generic;

public class PlayerInventory : Drawer
{
    private InventoryTransferUI inventory;
    private List<InventoryCell> playerInventory;

    public PlayerInventory()
    {
        this.inventory = new InventoryTransferUI();
        this.Add(inventory);

        playerInventory = new List<InventoryCell> { null };
    }

    public override void Update()
    {
        this.playerInventory[0] = Managers.MainCharacter.InventoryCell;
        this.inventory.UpdateInventories(this.playerInventory);
    }
}