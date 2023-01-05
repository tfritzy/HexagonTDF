using UnityEngine.UIElements;

public class PlayerInventory : UIPage
{
    private InventoryUI inventory;

    public PlayerInventory()
    {
        var modal = new Modal(800);
        this.Add(modal);

        modal.Add(new CraftingMenu());

        this.inventory = new InventoryUI();
        modal.Add(inventory);
    }

    public override void Update()
    {
        this.inventory.Update(Managers.MainCharacter.InventoryCell);
    }
}