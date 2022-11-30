using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildInputMode : InputMode
{
    public BuildingType SelectedBuildingType;

    public override void Interact(List<HexagonMono> hexes, List<Character> characters)
    {
        Debug.Log($"Build input mode interacts with {hexes.Count} hexes and {characters.Count} characters. I have build mode {SelectedBuildingType}");

        if (hexes.Count > 0 && SelectedBuildingType != BuildingType.Invalid)
        {
            BuildBuilding(hexes.First(), SelectedBuildingType);
        }
    }

    public void SelectBuildingType(BuildingType type)
    {
        this.SelectedBuildingType = type;
    }

    private bool CanBuildBuildingOnHex(Vector2Int pos, BuildingType building)
    {
        return building != BuildingType.Invalid && Managers.Board.GetBuilding(pos) == null;
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
    }

    public override void Update()
    {
    }
}