using System.Collections.Generic;
using UnityEngine;

public abstract class InputMode
{
    public abstract void OnDown(List<HexagonMono> hexes, List<Character> characters, int button);
    public abstract void OnDrag(List<HexagonMono> hexes, List<Character> characters);
    public abstract void OnUp(List<HexagonMono> hexes, List<Character> characters, int button, bool hasDragged);
    public virtual void OnHover(List<HexagonMono> hexes, List<Character> characters) { }
    public abstract void Update();
    public virtual void OnExit() { }
}