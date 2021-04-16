using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonFunctions : MonoBehaviour
{
    public Building Building;

    public void SelectBuilding()
    {
        if (Managers.Builder.SelectedBuilding == null)
        {
            Managers.Builder.SetSelectedBuilding(this, Building);
        }
        else
        {
            Managers.Builder.SetSelectedBuilding(this, null);
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
