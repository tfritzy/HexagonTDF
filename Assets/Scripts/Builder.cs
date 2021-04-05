using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
    public bool IsInBuildMode { get; private set; }
    public Building SelectedBuilding
    {
        get { return selectedBuilding; }
        private set
        {
            selectedBuilding = value;
            Managers.CameraControl.IsFrozen = value == null ? false : true;
            UnHighlightHexagon();
            ExitConfirmBuildMode();
            if (selectedBuilding != null)
            {
                selectBuildingTime = Time.unscaledTime;
            }
        }
    }

    private float selectBuildingTime;
    private Building selectedBuilding;
    private Hexagon highlightedHexagon;
    private ButtonFunctions responsibleButton;
    private GameObject buildingInst;
    private bool isInConfirmBuild;
    private GameObject acceptButton;
    private GameObject denyButton;

    void Start()
    {
        this.IsInBuildMode = false;
    }

    void Update()
    {
        if (IsInBuildMode)
        {
            HighlightHexagon();
        }
    }

    public void ToggleBuildMode()
    {
        IsInBuildMode = !IsInBuildMode;

        if (IsInBuildMode == false)
        {
            SelectedBuilding = null;
            UnHighlightHexagon();
            Managers.BuildButton.gameObject.SetActive(true);
            ExitConfirmBuildMode();
            Time.timeScale = 1f;
        }
        else
        {
            Managers.BuildButton.gameObject.SetActive(false);
            Time.timeScale = 0f;
        }
    }

    private void UnHighlightHexagon()
    {
        Destroy(buildingInst);
        highlightedHexagon?.SetMaterial(Constants.Materials.Normal);
        highlightedHexagon = null;

        RemoveHighlighting();
    }

    private void HighlightHexagon()
    {
        if (SelectedBuilding == null || isInConfirmBuild)
        {
            return;
        }

        HighlightHexagon(Helpers.FindHexByRaycast());

        if (Input.GetMouseButtonUp(0) && Time.unscaledTime - selectBuildingTime > .25f && highlightedHexagon.IsBuildable)
        {
            isInConfirmBuild = true;
            InstantiateAcceptAndDenyButtons();
        }
    }

    private void HighlightHexagon(Hexagon newPotentialHexagon)
    {
        if (newPotentialHexagon == null)
        {
            return;
        }

        if (newPotentialHexagon == highlightedHexagon)
        {
            return;
        }

        highlightedHexagon = newPotentialHexagon;

        CreateHighlightBuildingIfNeeded();
        buildingInst.transform.position = highlightedHexagon.transform.position;
        selectedBuilding.Position = highlightedHexagon.GridPosition;

        RemoveHighlighting();
        if (selectedBuilding.TryGetComponent<ResourceCollector>(out ResourceCollector collector))
        {
            HighlightCollectionHexagons(collector.CollectionRange, collector.CollectionTypes);
        }

        if (highlightedHexagon.IsBuildable)
        {
            buildingInst.SetMaterialsRecursively(Constants.Materials.BlueSeethrough);
        }
        else
        {
            buildingInst.SetMaterialsRecursively(Constants.Materials.RedSeethrough);
        }
    }

    private void CreateHighlightBuildingIfNeeded()
    {
        if (buildingInst == null)
        {
            buildingInst = Instantiate(SelectedBuilding.gameObject);
            Destroy(buildingInst.GetComponent<Building>());
        }
    }

    private void InstantiateAcceptAndDenyButtons()
    {
        float distanceBetweenButtons = 150;
        acceptButton = Instantiate(Prefabs.UIElements[UIElementType.Accept], Managers.Canvas);
        acceptButton.transform.position = Managers.Camera.WorldToScreenPoint(highlightedHexagon.transform.position) + new Vector3(150, distanceBetweenButtons / 2);

        denyButton = Instantiate(Prefabs.UIElements[UIElementType.Deny], Managers.Canvas);
        denyButton.transform.position = Managers.Camera.WorldToScreenPoint(highlightedHexagon.transform.position) + new Vector3(150, -distanceBetweenButtons / 2);
    }

    private void ExitConfirmBuildMode()
    {
        selectBuildingTime = Time.unscaledTime;
        isInConfirmBuild = false;
        Destroy(acceptButton);
        Destroy(denyButton);
    }

    public void AcceptConstructBuilding()
    {
        if (selectedBuilding.BuildCost.CanFulfill())
        {
            selectedBuilding.BuildCost.Deduct();
            Building building = Instantiate(selectedBuilding, highlightedHexagon.transform.position, new Quaternion()).GetComponent<Building>();
            building.Initialize(highlightedHexagon.GridPosition);
        }
        else
        {
            Debug.Log("Not enough resources to construct. Exiting build mode.");
        }

        UnHighlightHexagon();
        ExitConfirmBuildMode();
        this.SelectedBuilding = null;
        responsibleButton.RevertIcon();
    }

    public bool SetSelectedBuilding(ButtonFunctions responsibleButton, Building building)
    {
        if (building.BuildCost.CanFulfill() == false)
        {
            Debug.Log("Not enough resources");
            Debug.Log(building.BuildCost.ToString());
            return false;
        }

        this.SelectedBuilding = building;
        this.responsibleButton = responsibleButton;
        return true;
    }

    public void DenyConstructBuilding()
    {
        ExitConfirmBuildMode();
    }

    private Dictionary<Vector2Int, GameObject> highlightHexes;
    public void HighlightCollectionHexagons(int collectionRange, HashSet<HexagonType> collectionTypes)
    {
        if (highlightHexes == null || highlightHexes.Count == 0)
        {
            highlightHexes = new Dictionary<Vector2Int, GameObject>();
            List<Vector2Int> hexesInRange = Helpers.GetPointsInRange(selectedBuilding.Position, collectionRange);
            foreach (Vector2Int pos in hexesInRange)
            {
                highlightHexes[pos] = Instantiate(Prefabs.HighlightHex,
                    Managers.Map.Hexagons[pos.x, pos.y].transform.position + Vector3.up * .01f,
                    Prefabs.HighlightHex.transform.rotation,
                    null);
            }
        }

        foreach (Vector2Int pos in highlightHexes.Keys)
        {
            if (collectionTypes.Contains(Managers.Map.Hexagons[pos.x, pos.y].Type))
            {
                highlightHexes[pos].GetComponent<MeshRenderer>().material = Constants.Materials.Gold;
            }
            else
            {
                highlightHexes[pos].GetComponent<MeshRenderer>().material = Constants.Materials.RedSeethrough;
            }
        }
    }

    public void RemoveHighlighting()
    {
        if (highlightHexes == null)
        {
            return;
        }

        foreach (GameObject hex in highlightHexes.Values)
        {
            Destroy(hex);
        }

        highlightHexes = null;
    }
}
