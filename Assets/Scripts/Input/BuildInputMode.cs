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
    private Vector2Int previewPosition;

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

        if (this.State == BuildInputState.PreviewingBuilding)
        {
            ExitPreviewState();
        }

        this.CreatePreviewBuilding(hex, type);
        this.HighlightResourceHexes(hex, type);
        this.previewPosition = hex.GridPosition;
        this.previewHoverer = (BuildConfirmation)Managers.UI.ShowHoverer(
            Hoverer.BuildConfirmation,
            this.previewBuilding.transform);
        ((BuildConfirmation)previewHoverer).Init(
            () => BuildBuilding(hex, type),
            () => ExitPreviewState()
        );
        this.State = BuildInputState.PreviewingBuilding;
    }

    private void HighlightResourceHexes(HexagonMono hex, BuildingType type)
    {
        var building = this.previewBuilding.GetComponent<Building>();
        if (building.ResourceCollectionCell == null)
        {
            return;
        }

        foreach (Vector2Int pos in Helpers.GetHexesInRange(hex.GridPosition, 1))
        {
            var iHex = Managers.Board.GetHex(pos);

            if (building.ResourceCollectionCell.BiomesCollectedFrom.ContainsKey(iHex.Biome))
            {
                iHex.SetBorderMaterial(Prefabs.GetMaterial(MaterialType.Gold));
            }
        }
    }

    private void ResetHighlightedHexes(Vector2Int centerPos)
    {
        foreach (Vector2Int pos in Helpers.GetHexesInRange(centerPos, 1))
        {
            Managers.Board.GetHex(pos)?.ResetMaterial();
        }
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
        building.GetComponent<Building>().Setup();
        building.GetComponentInChildren<MeshRenderer>().material = Prefabs.GetMaterial(MaterialType.TransparentBlue);
    }

    private void ExitPreviewState()
    {
        GameObject.Destroy(previewBuilding);
        Managers.UI.HideHoverer(this.previewHoverer);
        ResetHighlightedHexes(this.previewPosition);
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