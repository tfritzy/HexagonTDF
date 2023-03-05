using System.Collections.Generic;

public class PlayerInventory : Drawer
{
    private InventoryTransferUI inventory;
    private List<InventoryCell> playerInventory;

    public PlayerInventory()
    {
        this.Add(new SquareButton(
            onClick: () => Managers.UI.OpenModal(ModalType.TransferItems),
            icon: Icons.GetUiIcon(UIIconType.Inventory)
        ));
        this.Add(new Divider());
        this.inventory = new InventoryTransferUI(showTitles: false);
        this.Add(inventory);

        playerInventory = new List<InventoryCell> { null };
    }

    public override void Update()
    {
        this.playerInventory[0] = Managers.MainCharacter.InventoryCell;
        this.inventory.UpdateInventories(this.playerInventory);
    }
}