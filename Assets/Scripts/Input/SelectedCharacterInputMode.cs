using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class SelectedCharacterInputMode : InputMode
{
    private Character SelectedCharacter;
    private GameObject selectedCharacterRing;

    public SelectedCharacterInputMode(Character character)
    {
        this.SelectedCharacter = character;
        this.selectedCharacterRing = GameObject.Instantiate(
            Managers.Prefabs.SelectedCharacterRing,
            character.transform.position + Vector3.up * .1f,
            Managers.Prefabs.SelectedCharacterRing.transform.rotation,
            character.transform);

        this.selectedCharacterRing.transform.localScale *=
            SelectedCharacter.GetComponent<CapsuleCollider>().radius * 2f;
    }

    public override void OnDown(List<HexagonMono> hexes, List<Character> characters, int button)
    {
    }

    public override void OnDrag(List<HexagonMono> hexes, List<Character> characters)
    {
    }

    public override void OnUp(
        List<HexagonMono> hexes,
        List<Character> characters,
        int button,
        bool hasDragged)
    {
        if (button == 0 && !hasDragged)
        {
            if (characters.Count > 0)
            {
                Managers.InputManager.OpenSelectedCharacterMode(characters[0]);
            }
            else
            {
                // Deselect current character.
                Managers.InputManager.SetGameInputMode();
            }
        }
    }

    public override void OnExit()
    {
        GameObject.Destroy(this.selectedCharacterRing);
    }

    public override void Update()
    {
        ListenToKeys();
    }

    private void ListenToKeys()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (Managers.UI.GetModal(ModalType.TransferItems).style.display == DisplayStyle.None)
            {
                Managers.UI.OpenModal(ModalType.TransferItems);
            }
            else
            {
                Managers.UI.CloseModal(ModalType.TransferItems);
            }
        }
    }
}