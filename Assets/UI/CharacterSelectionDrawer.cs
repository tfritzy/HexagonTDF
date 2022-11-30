using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class CharacterSelectionDrawer : Drawer
{
    private Label characterNameLabel;
    private VisualElement InventoryContainer;
    private List<InventoryUI> Inventories;

    public CharacterSelectionDrawer(VisualElement root) : base(root)
    {
        this.characterNameLabel = root.Q<Label>("CharacterName");
        
        this.InventoryContainer = root.Q<VisualElement>("Inventories");
        Inventories = new List<InventoryUI>();
    }

    public void Update(string characterName, List<InventoryCell> currentInventories)
    {
        this.characterNameLabel.text = characterName;

        while (Inventories.Count < currentInventories.Count)
        {
            InventoryUI inventoryUI = new InventoryUI();
            Inventories.Add(inventoryUI);
            InventoryContainer.Add(inventoryUI);
        }

        for (int i = 0; i < currentInventories.Count; i++)
        {
            Inventories[i].Update(currentInventories[i]);
        }

        // TODO: disable extra inventories.
    }
}