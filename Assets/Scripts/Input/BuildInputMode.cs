using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildInputMode : InputMode
{
    public BuildingType SelectedBuildingType;

    public override void Interact(List<HexagonMono> hexes, List<Character> characters)
    {
        Debug.Log($"Build input mode interacts with {hexes.Count} hexes and {characters.Count} characters");

        if (hexes.Count > 0 && SelectedBuildingType != BuildingType.Invalid)
        {
            BuildBuilding(hexes.First(), SelectedBuildingType);
        }
    }

    public void SelectBuildingType(string type)
    {
        if (Enum.TryParse<BuildingType>(type, out BuildingType parsedType))
        {
            this.SelectedBuildingType = parsedType;
        } else 
        {
            Debug.LogWarning("Tried to set the builder's selected type to unknown value " + type);
        }
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
            new Quaternion());
        Building building = buildingGO.GetComponent<Building>();
        building.Init(hex.GridPosition);
        Managers.Board.AddBuilding(hex.GridPosition, building);
    }
}