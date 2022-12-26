using System.Collections.Generic;
using UnityEngine;

public class GameInputMode : InputMode
{
    private Character selectedCharacter;

    public override void OnUp(List<HexagonMono> hexes, List<Character> characters, bool hasDragged)
    {
        if (characters.Count > 0 && !hasDragged)
        {
            selectedCharacter = characters[0];
            Managers.UI.ShowPage(Page.CharacterSelectionDrawer);
            UpdateSelectedCharacterUI();
        }

        Debug.Log($"Game input mode onUp interacts with {hexes.Count} hexes and {characters.Count} characters");
    }

    public override void OnDrag(List<HexagonMono> hexes, List<Character> characters)
    {
    }

    public override void OnDown(List<HexagonMono> hexes, List<Character> characters)
    {
        Debug.Log($"Game input mode onDown interacts with {hexes.Count} hexes and {characters.Count} characters");
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