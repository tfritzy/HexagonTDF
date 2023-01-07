using System.Collections.Generic;
using UnityEngine;

public class GameInputMode : InputMode
{
    public override void OnUp(List<HexagonMono> hexes, List<Character> characters, int button, bool hasDragged)
    {
        if (characters.Count > 0 && !hasDragged && button == 0)
        {
            Managers.InputManager.OpenSelectedCharacterMode(characters[0]);
        }
    }

    public override void OnDrag(List<HexagonMono> hexes, List<Character> characters)
    {
    }

    public override void OnDown(List<HexagonMono> hexes, List<Character> characters, int button)
    {
    }

    public override void Update()
    {
    }
}