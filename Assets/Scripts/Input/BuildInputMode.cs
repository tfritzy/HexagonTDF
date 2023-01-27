using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class BuildInputMode : InputMode
{
    public BuildingType SelectedBuildingType;
    public BuildInputState State { get; private set; }

    private ResourceCollectionIndicator resourceCollectionIndicator;
    private GameObject previewBuilding;
    private Character retargetingConveyorOf;
    private GameObject ArrowIndicator;

    public enum BuildInputState
    {
        Default,
        PreviewingBuilding,
        RetargetingConveyor,
    }

    public override void OnUp(List<HexagonMono> hexes, List<Character> characters, int button, bool hasDragged)
    {
        if (State == BuildInputState.RetargetingConveyor && retargetingConveyorOf != null)
        {
            Character newConveyor = characters.FirstOrDefault((Character c) => c.ConveyorCell != null);
            if (CanCharacterAcceptInput(retargetingConveyorOf, newConveyor))
            {
                retargetingConveyorOf.ConveyorCell.SwitchOutput(newConveyor.ConveyorCell);
            }

            ArrowIndicator.SetActive(false);
            State = BuildInputState.Default;
        }
        else if (State == BuildInputState.Default && !hasDragged)
        {
            if (hexes.Count > 0 && SelectedBuildingType != BuildingType.Invalid)
            {
                BuildBuilding(hexes.First(), SelectedBuildingType);
            }
        }
    }

    public override void OnHover(List<HexagonMono> hexes, List<Character> characters)
    {
        if (this.State == BuildInputState.PreviewingBuilding && hexes.Count > 0)
        {
            this.previewBuilding.transform.position = hexes.First().transform.position;
        }
    }

    public override void OnDrag(List<HexagonMono> hexes, List<Character> characters)
    {
        if (this.State == BuildInputState.RetargetingConveyor)
        {
            Character building = Managers.Board.GetBuilding(hexes.First().GridPosition);
            ArrowIndicator.transform.LookAt(hexes.First().transform);

            if (building != null && CanCharacterAcceptInput(retargetingConveyorOf, building))
            {
                ArrowIndicator.GetComponent<MeshRenderer>().material.color = Constants.Colors.Green;
            }
            else
            {
                ArrowIndicator.GetComponent<MeshRenderer>().material.color = Constants.Colors.Red;
            }
        }
    }

    public override void OnDown(List<HexagonMono> hexes, List<Character> characters, int button)
    {
        if (this.State == BuildInputState.PreviewingBuilding)
        {
            BuildBuilding(hexes.First(), this.SelectedBuildingType);
        }
        else
        {
            Character conveyor = characters.FirstOrDefault(
                        (Character character) =>
                            character.ConveyorCell != null &&
                            !character.ConveyorCell.IsTermination);

            if (conveyor != null)
            {
                this.State = BuildInputState.RetargetingConveyor;
                Managers.CameraControl.FrozenUntilMouseUp = true;
                this.retargetingConveyorOf = conveyor;
                if (this.ArrowIndicator == null)
                {
                    this.ArrowIndicator = GameObject.Instantiate(
                        Managers.Prefabs.RedArrow3D,
                        conveyor.transform.position,
                        new Quaternion());
                }
                else
                {
                    this.ArrowIndicator.transform.position = conveyor.transform.position;
                    this.ArrowIndicator.SetActive(true);
                }
            }
        }
    }

    private bool CanCharacterAcceptInput(Character source, Character target)
    {
        if (target?.ConveyorCell == null)
        {
            return false;
        }

        if (source.ConveyorCell == target.ConveyorCell)
        {
            return false;
        }

        if (target.ConveyorCell.Next == source.ConveyorCell)
        {
            return false;
        }

        if (target.ConveyorCell.IsTermination)
        {
            return false;
        }

        return !target.ConveyorCell.IsTermination;
    }

    public void SelectBuildingType(BuildingType type)
    {
        this.CreatePreviewBuilding(type);
        this.SelectedBuildingType = type;
        this.State = BuildInputState.PreviewingBuilding;
    }

    private void HighlightResourceHexes(HexagonMono hex, BuildingType type)
    {
        var building = this.previewBuilding.GetComponent<Building>();
        if (building.ResourceCollectionCell == null)
        {
            return;
        }


        if (building.ResourceCollectionCell.CurrentCollectionDetails != null)
        {
            this.resourceCollectionIndicator = (ResourceCollectionIndicator)Managers.UI.ShowHoverer(
                Hoverer.ResourceCollectionIndicator,
                this.previewBuilding.transform);

            this.resourceCollectionIndicator.Init(building.ResourceCollectionCell.CurrentCollectionDetails);
        }

        foreach (Vector2Int pos in building.ResourceCollectionCell.HexesCollectedFrom)
        {
            var iHex = Managers.Board.GetHex(pos);

            if (iHex == null)
            {
                continue;
            }
        }
    }

    private void ResetHighlightedHexes(Vector2Int centerPos)
    {
        Managers.Board.GetHex(centerPos)?.ResetMaterial();
        foreach (Vector2Int pos in Helpers.GetHexesInRange(centerPos, 1))
        {
            Managers.Board.GetHex(pos)?.ResetMaterial();
        }
    }

    private bool CanBuildBuildingOnHex(Vector2Int pos, BuildingType building)
    {
        return building != BuildingType.Invalid && Managers.Board.GetBuilding(pos) == null;
    }

    private void CreatePreviewBuilding(BuildingType type)
    {
        if (previewBuilding != null)
        {
            GameObject.Destroy(previewBuilding);
        }

        Building building = GameObject.Instantiate(Prefabs.GetBuilding(type)).GetComponent<Building>();
        Transform body = building.transform.Find("Body");
        body.SetParent(null);
        GameObject.Destroy(building);
        body.GetComponent<MeshRenderer>().material = Prefabs.GetMaterial(MaterialType.TransparentBlue);
        this.previewBuilding = body.gameObject;
    }

    private void ExitPreviewState()
    {
        GameObject.Destroy(previewBuilding);
        Managers.UI.HideHoverer(this.resourceCollectionIndicator);
        this.State = BuildInputState.Default;
    }

    private void BuildBuilding(HexagonMono hex, BuildingType type)
    {
        if (!CanBuildBuildingOnHex(hex.GridPosition, type))
        {
            return;
        }

        Building building = Managers.Board.InstantiateBuilding(hex.GridPosition, type);
        Managers.Board.AddBuilding(hex.GridPosition, building);

        ExitPreviewState();
    }

    public override void Update()
    {
    }
}