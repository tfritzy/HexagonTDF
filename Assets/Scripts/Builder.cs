﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Steps:
///  1. Select a hex by clicking. Highlight hex.
///     1.a click on other hex
///       1. Unhighlight hex goto 1.
///  2. Build menu flies in.
///  3. Select a building. Create highlight building.
///     3.a Click on other hex.
///       1. Unhighlight hex, destroy highlight building. goto 1.
///     3.b Select different building.
///       1. Destroy highlight building. goto 3.
///  4. Create confirm or deny buttons, and building stats page.
///     4.a Deny
///       1. Close menu, unselect hex, unhighlight hex.
///     4.b Accept
///       1. Create real building, unighlight hex, unselect hex, close menu.
/// </summary>
public class Builder : MonoBehaviour
{
    public Building SelectedBuilding
    {
        get { return selectedBuilding; }
        private set
        {
            selectedBuilding = value;
            SetCostPanels();
        }
    }

    private Building selectedBuilding;
    private HexagonMono highlightedHexagon;
    private ButtonFunctions responsibleButton;
    private GameObject buildingInst;
    private bool isInConfirmBuild;
    private GameObject confirmButtons;
    private Dictionary<ResourceType, Text> CostPanels;
    private GameObject menu;
    private bool CanOpenMenuThisFrame; // Used to stop menu from getting reopened after it got closed.

    void Start()
    {
        CostPanels = new Dictionary<ResourceType, Text>();
        foreach (ResourceType resourceType in Enum.GetValues(typeof(ResourceType)))
        {
            CostPanels[resourceType] = Managers.ResourceStore.transform.Find(resourceType.ToString()).Find("Cost").Find("Text").GetComponent<Text>();
        }
        SetCostPanels();

        menu = Prefabs.UIElements[UIElementType.AttackTowerBuildMenu];
        menu.SetActive(false);
    }

    private void UnHighlightHexagon()
    {
        Destroy(buildingInst);
        highlightedHexagon?.ResetMaterial();
        buildingInst = null;
        highlightedHexagon = null;
    }

    public void InformHexWasClicked(HexagonMono hex)
    {
        SetBuildTargetHex(Helpers.FindHexByRaycast(Input.mousePosition));
    }

    private void SetBuildTargetHex(HexagonMono newPotentialHexagon)
    {
        if (newPotentialHexagon == null)
        {
            return;
        }

        if (newPotentialHexagon == highlightedHexagon)
        {
            return;
        }

        this.SelectedBuilding = null;
        UnHighlightHexagon();

        menu.SetActive(true);
        highlightedHexagon = newPotentialHexagon;
        highlightedHexagon.SetMaterial(Constants.Materials.Gold);
    }

    private void CreateHighlightBuilding(bool isBuildable)
    {
        if (buildingInst == null)
        {
            buildingInst = Instantiate(SelectedBuilding.gameObject);
            Destroy(buildingInst.GetComponent<Building>());
        }

        buildingInst.transform.position = highlightedHexagon.transform.position;
        selectedBuilding.GridPosition = highlightedHexagon.GridPosition;

        if (isBuildable)
        {
            buildingInst.SetMaterialsRecursively(Constants.Materials.BlueSeethrough);
        }
        else
        {
            buildingInst.SetMaterialsRecursively(Constants.Materials.RedSeethrough);
        }

        if (selectedBuilding is AttackTower)
        {
            ((AttackTower)selectedBuilding).CreateRangeCircle(buildingInst.transform);
        }
    }

    private void ExitConfirmBuildMode()
    {
        isInConfirmBuild = false;
    }

    public void AcceptConstructBuilding()
    {
        Vector2Int pos = highlightedHexagon.GridPosition;
        if (selectedBuilding.BuildCost.CanFulfill() && Managers.Board.IsBuildable(highlightedHexagon.GridPosition))
        {
            selectedBuilding.BuildCost.Deduct();
            Building building = Instantiate(selectedBuilding, highlightedHexagon.transform.position, new Quaternion()).GetComponent<Building>();
            building.Initialize(highlightedHexagon.GridPosition);
        }
        else
        {
            Debug.Log("Not enough resources, or invalid position");
        }

        ExitBuildDialog();
    }

    public void SetSelectedBuilding(ButtonFunctions responsibleButton, Building building)
    {
        if (this.SelectedBuilding != building)
        {
            Destroy(buildingInst);
            buildingInst = null;
        }

        this.SelectedBuilding = building;
        this.responsibleButton = responsibleButton;

        bool isBuildable = Managers.Board.IsBuildable(highlightedHexagon.GridPosition);

        if (isBuildable)
        {
            // TODO: open building stats page.
        }

        CreateHighlightBuilding(isBuildable);
    }

    private void SetCostPanels()
    {
        foreach (Text panel in CostPanels.Values)
        {
            panel.transform.parent.gameObject.SetActive(false);
        }

        if (selectedBuilding != null)
        {
            foreach (ResourceType resource in selectedBuilding.BuildCost.Costs.Keys)
            {
                CostPanels[resource].transform.parent.gameObject.SetActive(true);
                CostPanels[resource].text = $"-{selectedBuilding.BuildCost.Costs[resource].ToString()}";
            }
        }
    }

    public void ExitBuildDialog()
    {
        UnHighlightHexagon();
        this.SelectedBuilding = null;
        ExitConfirmBuildMode();
        this.menu?.SetActive(false);
        CanOpenMenuThisFrame = false;
    }
}