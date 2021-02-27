using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildButton : MonoBehaviour
{
    private GameObject menu;

    public void Click()
    {
        Managers.Builder.ToggleBuildMode();
        if (Managers.Builder.IsInBuildMode)
        {
            menu = Instantiate(Prefabs.AttackTowerBuildMenu, Managers.Canvas);
        }
        else
        {
            Destroy(menu);
        }
    }
}
