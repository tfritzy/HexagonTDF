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
            Managers.InputManager.OpenSelectedCharacterMode(selectedCharacter);
        }
    }

    public override void OnDrag(List<HexagonMono> hexes, List<Character> characters)
    {
    }

    public override void OnDown(List<HexagonMono> hexes, List<Character> characters)
    {
    }

    public override void Update()
    {
    }
}