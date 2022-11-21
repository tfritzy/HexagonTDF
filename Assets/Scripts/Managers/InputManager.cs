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
    private BuildInputMode buildInputMode;
    private GameInputMode gameInputMode;

    void Awake()
    {
        this.buildInputMode = new BuildInputMode();
        this.gameInputMode = new GameInputMode();
        this.CurrentMode = gameInputMode;
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

    public void SwapInputMode()
    {
        if (CurrentMode is BuildInputMode)
        {
            CurrentMode = this.gameInputMode;
        }
        else
        {
            this.CurrentMode = this.buildInputMode;
        }
    }

    public void SelectBuilding(string buildingType)
    {
        this.buildInputMode.SelectBuildingType(buildingType);
    }
}
