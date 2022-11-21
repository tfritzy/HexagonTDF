using System.Collections.Generic;
using UnityEngine;

public class BuildInputMode : InputMode
{
    public override void Interact(List<HexagonMono> hexes, List<Character> characters)
    {
        Debug.Log($"Build input mode interacts with {hexes.Count} hexes and {characters.Count} characters");
    }
}