using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public bool hasDragged;
    public bool IsFingerHeldDown;
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
        if (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
        {
            ShootRayCast();
        }

        if (Managers.CameraControl.IsDragging())
        {
            hasDragged = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            hasDragged = false;
        }

        this.CurrentMode.Update();
    }

    private void ShootRayCast()
    {
        Vector3? inputPos = null;

        if (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
        {
            inputPos = Input.mousePosition;
        }
        else if (Input.touchCount > 0)
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

        var hits = RaycastAndInteract(inputPos, Constants.Layers.Hexagons | Constants.Layers.Units);
        if (hits == null)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            this.CurrentMode.OnDown(hits.Value.Hexes, hits.Value.Characters);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            this.CurrentMode.OnUp(hits.Value.Hexes, hits.Value.Characters, hasDragged);
        }
        else
        {
            this.CurrentMode.OnDrag(hits.Value.Hexes, hits.Value.Characters);
        }
    }

    struct RaycastHits
    {
        public List<Character> Characters;
        public List<HexagonMono> Hexes;
        public List<Vector3> HitPoints;
    };

    private RaycastHits? RaycastAndInteract(Vector3? inputPos, int layer)
    {
        if (Helpers.IsPointerOverUI())
        {
            return null;
        }

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
        characters.AddRange(hexes.Select((HexagonMono hex) => Managers.Board.GetBuilding(hex.GridPosition)));
        hexes.RemoveAll((HexagonMono i) => i == null);
        characters.RemoveAll((Character i) => i == null);
        return new RaycastHits
        {
            Characters = characters,
            Hexes = hexes,
            HitPoints = hits.Select((RaycastHit hit) => hit.point).ToList(),
        };
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public void SwitchMode()
    {
        this.CurrentMode.OnExit();
    }

    public void OpenSelectedCharacterMode(Character character)
    {
        this.CurrentMode = new SelectedCharacterInputMode(character);
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
