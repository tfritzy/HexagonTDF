using System.Collections.Generic;
using UnityEngine;

public abstract class InputMode
{
    public abstract void OnDown(List<HexagonMono> hexes, List<Character> characters);
    public abstract void OnDrag(List<HexagonMono> hexes, List<Character> characters);
    public abstract void OnUp(List<HexagonMono> hexes, List<Character> characters, bool hasDragged);
    public abstract void Update();
}