using System.Collections.Generic;
using UnityEngine;

public class GameInputMode : InputMode
{
    private Character selectedCharacter;

    public override void Interact(List<HexagonMono> hexes, List<Character> characters)
    {
        if (characters.Count > 0)
        {
            selectedCharacter = characters[0];
            UpdateSelectedCharacterUI();
        }

        Debug.Log($"Game input mode interacts with {hexes.Count} hexes and {characters.Count} characters");
    }

    public override void Update()
    {
        UpdateSelectedCharacterUI();
    }

    private void UpdateSelectedCharacterUI()
    {
        if (selectedCharacter != null)
        {
            CharacterSelectionDrawer drawer = (CharacterSelectionDrawer)Managers.UI.GetPage(Page.CharacterSelectionDrawer);
            Character character = selectedCharacter;
            List<InventoryCell> cells = new List<InventoryCell>();
            if (character.ResourceCollectionCell != null)
            {
                cells.Add(character.ResourceCollectionCell.Inventory);
            }

            if (character.ResourceProcessingCell != null)
            {
                cells.Add(character.ResourceProcessingCell.InputInventory);
                cells.Add(character.ResourceProcessingCell.ProcessingInventory);
                cells.Add(character.ResourceProcessingCell.OutputInventory);
            }

            drawer.Update(selectedCharacter.name, cells);
            Managers.UI.ShowPage(Page.CharacterSelectionDrawer);
        }
    }
}