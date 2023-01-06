using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class CharacterSelectionDrawer : Drawer
{
    private Label characterNameLabel;
    private VisualElement InventoryContainer;
    private List<InventoryUI> Inventories;
    private Character selectedCharacter;
    private Button destroyButton;

    public CharacterSelectionDrawer()
    {
        this.characterNameLabel = new Label();
        this.characterNameLabel.AddToClassList("heading-1");
        this.Add(characterNameLabel);

        this.InventoryContainer = new VisualElement();
        this.Add(InventoryContainer);
        Inventories = new List<InventoryUI>();

        var closeButton = new Button();
        closeButton.AddToClassList("floating-circle-button");
        closeButton.clicked += () => Managers.UI.ShowPage(Page.ActionDrawer);
        this.Add(closeButton);

        this.destroyButton = new Button();
        destroyButton.clicked += () =>
        {
            Managers.Board.DestroyBuilding((Building)this.selectedCharacter);
            Managers.UI.Back();
        };
        this.InventoryContainer.Add(destroyButton);
    }

    public void Update(string characterName, Character selectedCharacter)
    {
        UpdateInventories(selectedCharacter);
        SetupDestroyButton(selectedCharacter);
        this.selectedCharacter = selectedCharacter;
    }

    private void SetupDestroyButton(Character character)
    {
        if (character != this.selectedCharacter)
        {
            if (character is Building)
            {
                destroyButton.style.display = DisplayStyle.Flex;
            }
            else
            {
                destroyButton.style.display = DisplayStyle.None;
            }
        }
    }

    private void UpdateInventories(Character character)
    {
        List<InventoryCell> inventories = new List<InventoryCell>();
        if (character.InventoryCell != null)
        {
            inventories.Add(character.InventoryCell);
        }

        if (character.ResourceProcessingCell != null)
        {
            inventories.Add(character.ResourceProcessingCell.InputInventory);
            inventories.Add(character.ResourceProcessingCell.ProcessingInventory);
            inventories.Add(character.ResourceProcessingCell.OutputInventory);
        }


        this.characterNameLabel.text = character.name;

        while (Inventories.Count < inventories.Count)
        {
            InventoryUI inventoryUI = new InventoryUI();
            Inventories.Add(inventoryUI);
            InventoryContainer.Add(inventoryUI);
        }

        int i;
        for (i = 0; i < inventories.Count; i++)
        {
            Inventories[i].Update(inventories[i]);
            Inventories[i].style.display = DisplayStyle.Flex;
        }

        while (i < Inventories.Count)
        {
            Inventories[i].style.display = DisplayStyle.None;
            i += 1;
        }
    }
}