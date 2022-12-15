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
            Managers.UI.ShowPage(Page.CharacterSelectionDrawer);
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

            drawer.Update(selectedCharacter.Name, selectedCharacter);
        }
    }
}