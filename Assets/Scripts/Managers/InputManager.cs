using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public bool DisabledByScroll;
    public bool IsFingerHeldDown;
    public static event EventHandler InputWasMade;
    public InputMode CurrentMode;
    public BuildInputMode BuildMode;
    public GameInputMode GameMode;

    void Awake()
    {
        this.BuildMode = new BuildInputMode();
        this.GameMode = new GameInputMode();
        this.CurrentMode = GameMode;

#if UNITY_EDITOR
        Cursor.visible = true;
#endif
    }

    void Update()
    {
        if (DisabledByScroll == false)
        {
            ShootRayCast();
        }

        if (Managers.CameraControl.IsDragging())
        {
            DisabledByScroll = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            DisabledByScroll = false;
        }

        this.CurrentMode.Update();
    }

    private void ShootRayCast()
    {
        Vector3? inputPos = null;

        if (Input.GetMouseButtonUp(0))
        {
            inputPos = Input.mousePosition;
        }
        else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            inputPos = Input.GetTouch(0).position;
        }
        if (!inputPos.HasValue)
        {
            return;
        }

        if (IsPointerOverUIObject())
        {
            return;
        }

        RaycastAndInteract(inputPos, Constants.Layers.Hexagons | Constants.Layers.Characters);
    }

    private void RaycastAndInteract(Vector3? inputPos, int layer)
    {
        Ray ray = Managers.Camera.ScreenPointToRay(inputPos.Value);
        RaycastHit[] hits = Physics.RaycastAll(
            ray,
            100f,
            layer,
            QueryTriggerInteraction.Collide);
        Array.Sort(hits, (RaycastHit h1, RaycastHit h2) => h1.distance.CompareTo(h2.distance));
        
        List<HexagonMono> hexes =
            hits.Select((RaycastHit hit) => hit.collider.transform.parent?.gameObject.GetComponent<HexagonMono>())
            .ToList();
        List<Character> characters =
            hits.Select((RaycastHit hit) => hit.collider.gameObject.GetComponent<Character>())
            .ToList();
        hexes.RemoveAll((HexagonMono i) => i == null);
        characters.RemoveAll((Character i) => i == null);
        this.CurrentMode.Interact(hexes, characters);
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public void OpenBuildMode()
    {
        this.CurrentMode = this.BuildMode;
    }

    public void SetGameInputMode()
    {
        this.CurrentMode = this.GameMode;
    }
}
