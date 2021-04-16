using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonFunctions : MonoBehaviour
{
    public Building Building;

    public void SelectBuilding()
    {
        if (Managers.Builder.SelectedBuilding?.Type == Building.Type)
        {
            Managers.Builder.SetSelectedBuilding(this, null);
        }
        else
        {
            Managers.Builder.SetSelectedBuilding(this, Building);
        }
    }

    public void AcceptBuildingConstruction()
    {
        Managers.Builder.AcceptConstructBuilding();
    }

    public void DenyBuildingConstruction()
    {
        Managers.Builder.DenyConstructBuilding();
    }
}
