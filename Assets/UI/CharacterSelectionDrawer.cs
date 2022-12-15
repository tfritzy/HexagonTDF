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


    public CharacterSelectionDrawer(VisualElement root) : base(root)
    {
        this.characterNameLabel = root.Q<Label>("CharacterName");

        this.InventoryContainer = root.Q<VisualElement>("Inventories");
        Inventories = new List<InventoryUI>();

        root.Q<Button>("Close").clicked += () => Managers.UI.ShowPage(Page.ActionDrawer);
        destroyButton = root.Q<Button>("Destroy");
        destroyButton.clicked += () =>
        {
            Managers.Board.DestroyBuilding((Building)this.selectedCharacter);
            Managers.UI.Back();
        };
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
        if (character.ResourceCollectionCell != null)
        {
            inventories.Add(character.ResourceCollectionCell.Inventory);
        }

        if (character.ResourceProcessingCell != null)
        {
            inventories.Add(character.ResourceProcessingCell.InputInventory);
            inventories.Add(character.ResourceProcessingCell.ProcessingInventory);
            inventories.Add(character.ResourceProcessingCell.OutputInventory);
        }

        if (character.InventoryCell != null)
        {
            inventories.Add(character.InventoryCell);
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