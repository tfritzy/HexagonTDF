using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildButton : MonoBehaviour
{
    private GameObject menu;

    public void Click()
    {
        Destroy(menu);
        Managers.Builder.ToggleBuildMode();
        if (Managers.Builder.IsInBuildMode)
        {
            menu = Instantiate(Prefabs.AttackTowerBuildMenu, Managers.Canvas);
        }
    }
}
