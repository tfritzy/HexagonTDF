using System.Collections.Generic;

public class PlayerInventory : UIPage
{
    private InventoryTransferUI inventory;
    private List<InventoryCell> playerInventory;

    public PlayerInventory()
    {
        var modal = new Modal(800, "Your inventory");
        this.Add(modal);

        this.inventory = new InventoryTransferUI();
        modal.Add(inventory);

        playerInventory = new List<InventoryCell> { null };
    }

    public override void Update()
    {
        this.playerInventory[0] = Managers.MainCharacter.InventoryCell;
        this.inventory.UpdateInventories(this.playerInventory);
    }
}