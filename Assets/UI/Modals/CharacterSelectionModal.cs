using System.Collections.Generic;

public class CharacterSelectionModal : Modal
{
    private Character renderedSourceCharacter;
    private Character renderedTargetCharacter;
    private List<InventoryCell> renderedInventories;
    private InventoryTransferUI transferUI;

    public CharacterSelectionModal(int width) : base(width)
    {
        this.transferUI = new InventoryTransferUI();
        this.Add(this.transferUI);
    }

    public void Update(Character sourceChar, Character targetChar)
    {
        this.transferUI.UpdateInventories(GetInventories(sourceChar, targetChar));
    }

    private List<InventoryCell> GetInventories(Character source, Character target)
    {
        if (renderedSourceCharacter == source && renderedTargetCharacter == target)
        {
            return renderedInventories;
        }

        this.renderedSourceCharacter = source;
        this.renderedTargetCharacter = target;
        List<InventoryCell> inventories = new List<InventoryCell>();

        if (source.InventoryCell != null)
        {
            inventories.Add(source.InventoryCell);
        }

        if (target.InventoryCell != null)
        {
            inventories.Add(target.InventoryCell);
        }

        this.renderedInventories = inventories;

        return inventories;
    }
}