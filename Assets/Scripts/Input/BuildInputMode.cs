using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class BuildInputMode : InputMode
{
    public BuildingType SelectedBuildingType;
    public BuildInputState State { get; private set; }
    private ResourceCollectionIndicator resourceCollectionIndicator;
    private List<Building> previewBuildings = new List<Building>();

    public enum BuildInputState
    {
        Default,
        PlacingBuilding, // Waiting for building placement.
        Previewing, // Waiting for mouse up event.
    }

    public override void OnUp(List<HexagonMono> hexes, List<Character> characters, int button, bool hasDragged)
    {
        if (this.State == BuildInputState.Previewing)
        {
            if (hexes.Count > 0 && SelectedBuildingType != BuildingType.Invalid)
            {
                foreach (Building building in previewBuildings)
                {
                    Managers.Board.DestroyBuilding(building);
                    BuildBuilding(building);
                }

                previewBuildings = new List<Building>();
                this.State = BuildInputState.PlacingBuilding;
            }
        }
    }

    public override void OnHover(List<HexagonMono> hexes, List<Character> characters)
    {
        if (this.State == BuildInputState.PlacingBuilding && hexes.Count > 0)
        {
            if (this.previewBuildings.Count > 0)
            {
                foreach (Building building in previewBuildings)
                {
                    Managers.Board.DestroyBuilding(building);
                }
                this.previewBuildings = new List<Building>();
            }

            if (Managers.Board.GetBuilding(hexes.First().GridPosition) == null)
            {
                this.CreatePreviewBuilding(SelectedBuildingType, hexes.First().GridPosition);
            }
        }
    }

    public override void OnDrag(List<HexagonMono> hexes, List<Character> characters)
    {
        if (this.State == BuildInputState.Previewing)
        {
            if (hexes.Count > 0 && Managers.Board.GetBuilding(hexes.First().GridPosition) == null)
            {
                this.CreatePreviewBuilding(SelectedBuildingType, hexes.First().GridPosition);
            }
        }
    }

    public override void OnDown(List<HexagonMono> hexes, List<Character> characters, int button)
    {
        if (State == BuildInputState.PlacingBuilding)
        {
            this.State = BuildInputState.Previewing;
        }
    }

    public void SelectBuildingType(BuildingType type)
    {
        this.SelectedBuildingType = type;
        this.State = BuildInputState.PlacingBuilding;
    }

    private void CreatePreviewBuilding(BuildingType type, Vector2Int gridPos)
    {
        if (!CanBuildBuildingOnHex(gridPos, type))
        {
            return;
        }

        Helpers.WorldToChunkPos(gridPos, out Vector2Int chunkIndex, out Vector3Int subPos);
        HexagonMono hex = Managers.Board.World.GetTopHexBody(chunkIndex, subPos.x, subPos.y);

        if (hex != null)
        {
            Building building = Managers.Board.BuildBuilding(type, hex.GridPosition);
            building.MarkPreview();
            this.previewBuildings.Add(building);
        }
    }

    private void ExitPreviewState()
    {
        foreach (Building building in previewBuildings)
        {
            Managers.Board.DestroyBuilding(building);
        }
        this.previewBuildings = new List<Building>();

        Managers.UI.HideHoverer(this.resourceCollectionIndicator);
        this.State = BuildInputState.Default;
    }

    private bool CanBuildBuildingOnHex(Vector2Int pos, BuildingType building)
    {
        if (building == BuildingType.Invalid)
        {
            return false;
        }

        return Managers.Board.GetBuilding(pos) == null || Managers.Board.GetBuilding(pos).IsPreview;
    }

    private void BuildBuilding(Building previewBuilding)
    {
        if (!CanBuildBuildingOnHex(previewBuilding.GridPosition, previewBuilding.Type))
        {
            return;
        }

        Managers.Board.BuildBuilding(previewBuilding.Type, previewBuilding.GridPosition);
    }

    private void ListenToKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (this.previewBuildings.Count > 0)
            {
                foreach (Building building in previewBuildings)
                {
                    Managers.Board.DestroyBuilding(building);
                }
                this.previewBuildings = new List<Building>();
                this.State = BuildInputState.PlacingBuilding;
            }
            else
            {
                Managers.UI.ShowPage(Page.ActionDrawer);
                Managers.InputManager.SetGameInputMode();
                ExitPreviewState();
            }
        }
    }

    public override void Update()
    {
        ListenToKeyInput();
    }

    public override void OnExit()
    {
        base.OnExit();
        this.ExitPreviewState();
    }
}