using System.Collections.Generic;
using UnityEngine.UIElements;

public class CharacterSelectionModal : Modal
{
    private Character renderedSourceCharacter;
    private Character renderedTargetCharacter;
    private List<InventoryCell> renderedInventories;
    private InventoryTransferUI transferUI;
    private ConstructionRequirements constructionRequirements;

    public CharacterSelectionModal() : base(800, "Transfer items")
    {
        this.constructionRequirements = new ConstructionRequirements();
        this.Add(this.constructionRequirements);
        this.constructionRequirements.style.display = DisplayStyle.None;

        this.transferUI = new InventoryTransferUI();
        this.Add(this.transferUI);
    }

    public void Update(Character sourceChar, Character targetChar)
    {
        this.UpdateConstructionNeeds(targetChar);
        this.transferUI.UpdateInventories(GetInventories(sourceChar, targetChar));
    }

    private void UpdateConstructionNeeds(Character targetChar)
    {
        if (targetChar is Building && ((Building)targetChar).NeedsConstruction)
        {
            if (this.constructionRequirements.style.display == DisplayStyle.None)
            {
                this.constructionRequirements.style.display = DisplayStyle.Flex;
            }

            this.constructionRequirements.Update((Building)targetChar);
        }
        else if (this.constructionRequirements.style.display == DisplayStyle.Flex)
        {
            this.constructionRequirements.style.display = DisplayStyle.None;
        }
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

        if (target.InventoryCell != null)
        {
            inventories.Add(target.InventoryCell);
        }

        if (source.InventoryCell != null)
        {
            inventories.Add(source.InventoryCell);
        }

        this.renderedInventories = inventories;

        return inventories;
    }
}