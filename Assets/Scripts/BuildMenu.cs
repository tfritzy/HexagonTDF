using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BuildMenu : MonoBehaviour
{
    public abstract List<BuildingType> BuildingTypes { get; }

    private const int NumButtons = 9;
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
            Button button = this.transform.Find("Button" + i).GetComponent<Button>();
            button.transform.Find("Icon").GetComponent<Image>().sprite = buildings[i].Icon;
            button.GetComponent<ButtonFunctions>().Building = buildings[i];
        }

        for (; i < NumButtons; i++)
        {
            Button button = this.transform.GetChild(i).GetComponent<Button>();
            button.gameObject.SetActive(false);
        }
    }

    public void Close()
    {
        Managers.Builder.ExitBuildDialog();
        this.gameObject.SetActive(false);
    }
}
