using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class BuildInputMode : InputMode
{
    public BuildingType SelectedBuildingType;
    public BuildInputState State { get; private set; }

    private BuildConfirmation previewHoverer;
    private GameObject previewBuilding;

    public enum BuildInputState
    {
        Default,
        PreviewingBuilding,
    }

    public override void Interact(List<HexagonMono> hexes, List<Character> characters)
    {
        Debug.Log($"Build input mode interacts with {hexes.Count} hexes and {characters.Count} characters. I have build mode {SelectedBuildingType}");

        if (hexes.Count > 0 && SelectedBuildingType != BuildingType.Invalid)
        {
            OpenBuildConfirmation(hexes.First(), SelectedBuildingType);
        }
    }

    public void SelectBuildingType(BuildingType type)
    {
        this.SelectedBuildingType = type;
    }

    public void OpenBuildConfirmation(HexagonMono hex, BuildingType type)
    {
        if (!CanBuildBuildingOnHex(hex.GridPosition, type))
        {
            return;
        }

        this.CreatePreviewBuilding(hex, type);
        this.previewHoverer = (BuildConfirmation)Managers.UI.ShowHoverer(Hoverer.BuildConfirmation, this.previewBuilding.transform);
        ((BuildConfirmation)previewHoverer).Init(
            () => BuildBuilding(hex, type),
            () => ExitPreviewState()
        );
        this.State = BuildInputState.PreviewingBuilding;
    }

    private bool CanBuildBuildingOnHex(Vector2Int pos, BuildingType building)
    {
        return building != BuildingType.Invalid && Managers.Board.GetBuilding(pos) == null;
    }

    private void CreatePreviewBuilding(HexagonMono hex, BuildingType type)
    {
        if (previewBuilding != null)
        {
            GameObject.Destroy(previewBuilding);
        }

        previewBuilding = GameObject.Instantiate(
            Prefabs.GetBuilding(type),
            hex.transform.position,
            Prefabs.GetBuilding(type).transform.rotation);
        Building building = previewBuilding.GetComponent<Building>();
        building.GetComponentInChildren<MeshRenderer>().material = Prefabs.GetMaterial(MaterialType.TransparentBlue);
    }

    private void ExitPreviewState()
    {
        GameObject.Destroy(previewBuilding);
        Managers.UI.HideHoverer(this.previewHoverer);
        this.State = BuildInputState.Default;
    }

    private void BuildBuilding(HexagonMono hex, BuildingType type)
    {
        if (!CanBuildBuildingOnHex(hex.GridPosition, type))
        {
            return;
        }

        var buildingGO = GameObject.Instantiate(
            Prefabs.GetBuilding(type),
            hex.transform.position,
            Prefabs.GetBuilding(type).transform.rotation);
        Building building = buildingGO.GetComponent<Building>();
        building.Init(hex.GridPosition);
        Managers.Board.AddBuilding(hex.GridPosition, building);

        ExitPreviewState();
    }

    public override void Update()
    {
        if (this.State == BuildInputState.PreviewingBuilding)
        {
            this.previewHoverer.Update();
        }
    }
}