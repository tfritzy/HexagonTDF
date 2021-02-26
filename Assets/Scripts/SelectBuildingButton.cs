using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectBuildingButton : MonoBehaviour
{
    public Building Building;

    public void Click()
    {
        if (Managers.Builder.SelectedBuilding == null)
        {
            Managers.Builder.SelectedBuilding = Building;
        }
        else
        {
            Managers.Builder.SelectedBuilding = null;
        }
    }
}
