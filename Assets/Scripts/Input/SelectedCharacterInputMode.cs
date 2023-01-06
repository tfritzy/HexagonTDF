using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectedCharacterInputMode : InputMode
{
    private Character SelectedCharacter;
    private GameObject selectedCharacterRing;

    public SelectedCharacterInputMode(Character character)
    {
        this.SelectedCharacter = character;
        Managers.UI.ShowPage(Page.CharacterSelectionModal);
        this.selectedCharacterRing = GameObject.Instantiate(
            Managers.Prefabs.SelectedCharacterRing,
            character.transform.position + Vector3.up * .1f,
            Managers.Prefabs.SelectedCharacterRing.transform.rotation,
            character.transform);

        this.selectedCharacterRing.transform.localScale *=
            SelectedCharacter.GetComponent<CapsuleCollider>().radius * 2f;
    }

    public override void OnDown(List<HexagonMono> hexes, List<Character> characters)
    {
    }

    public override void OnDrag(List<HexagonMono> hexes, List<Character> characters)
    {
    }

    public override void OnUp(List<HexagonMono> hexes, List<Character> characters, bool hasDragged)
    {
        if (!hasDragged && hexes.Count > 0)
        {
            SelectedCharacter.SelectedClickHex(hexes.First().GridPosition);
        }
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        if (SelectedCharacter != null)
        {
            CharacterSelectionModal modal = (CharacterSelectionModal)Managers.UI.GetPage(Page.CharacterSelectionModal);
            Character character = SelectedCharacter;
            modal.Update(Managers.MainCharacter, SelectedCharacter);
        }
    }
}