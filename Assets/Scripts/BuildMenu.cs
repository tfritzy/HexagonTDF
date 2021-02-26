using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BuildMenu : MonoBehaviour
{
    public abstract List<BuildingType> BuildingTypes { get; }

    private List<Building> buildings;

    void Start()
    {
        Setup();
    }

    public void Setup()
    {
        buildings = new List<Building>();

        foreach (BuildingType buildingType in BuildingTypes)
        {
            buildings.Add(Prefabs.Buildings[buildingType].GetComponent<Building>());
        }

        int i = 0;
        for (i = 0; i < BuildingTypes.Count; i++)
        {
            Button button = this.transform.GetChild(i).GetComponent<Button>();
            button.transform.Find("Image").GetComponent<Image>().sprite = buildings[i].Icon;
            button.GetComponent<SelectBuildingButton>().Building = buildings[i];
        }

        for (; i < this.transform.childCount; i++)
        {
            Button button = this.transform.GetChild(i).GetComponent<Button>();
            button.gameObject.SetActive(false);
        }
    }
}
