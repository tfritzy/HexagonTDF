using System.Collections.Generic;

public abstract class InputMode
{
    public abstract void Interact(List<HexagonMono> hexes, List<Character> characters);
}