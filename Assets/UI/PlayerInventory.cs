using System.Collections.Generic;

public class PlayerInventory : UIPage
{
    private InventoryTransferUI inventory;
    private List<InventoryCell> playerInventory;

    public PlayerInventory()
    {
        var modal = new Modal(800, "Your inventory");
        this.Add(modal);

        this.Add(new InventoryTransferUI());
        modal.Add(inventory);

        playerInventory = new List<InventoryCell> { Managers.MainCharacter.InventoryCell };
    }

    public override void Update()
    {
        this.inventory.UpdateInventories(playerInventory);
    }
}