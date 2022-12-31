using System.Collections.Generic;
using UnityEngine;

public class SelectedCharacterInputMode : InputMode
{
    private Character SelectedCharacter;
    private GameObject selectedCharacterRing;

    public SelectedCharacterInputMode(Character character)
    {
        this.SelectedCharacter = character;
        Managers.UI.ShowPage(Page.CharacterSelectionDrawer);
        this.selectedCharacterRing = GameObject.Instantiate(
            Managers.Prefabs.SelectedCharacterRing,
            character.transform.position + Vector3.up * .1f,
            Managers.Prefabs.SelectedCharacterRing.transform.rotation,
            character.transform);
    }

    public override void OnDown(List<HexagonMono> hexes, List<Character> characters)
    {
        Debug.Log("OnDown in selected char");
    }

    public override void OnDrag(List<HexagonMono> hexes, List<Character> characters)
    {
        Debug.Log("OnDrag in selected char");
    }

    public override void OnUp(List<HexagonMono> hexes, List<Character> characters, bool hasDragged)
    {
        Debug.Log("OnUp in selected char");
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        if (SelectedCharacter != null)
        {
            CharacterSelectionDrawer drawer = (CharacterSelectionDrawer)Managers.UI.GetPage(Page.CharacterSelectionDrawer);
            Character character = SelectedCharacter;
            drawer.Update(SelectedCharacter.Name, SelectedCharacter);
        }
    }
}