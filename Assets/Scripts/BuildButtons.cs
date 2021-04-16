using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildButtons : MonoBehaviour
{
    private GameObject menu;

    public void EnterAttackTowerMenu()
    {
        CreateMenu(Prefabs.UIElements[UIElementType.AttackTowerBuildMenu]);
    }

    public void CreateBaseBuilderMenu()
    {
        CreateMenu(Prefabs.UIElements[UIElementType.BaseBuilderMenu]);
    }

    private void CreateMenu(GameObject menuPrefab)
    {
        if (menu != null)
        {
            menu.GetComponent<BuildMenu>().Close();
        }

        Managers.Builder.ToggleBuildMode();
        if (Managers.Builder.IsInBuildMode)
        {
            menu = Instantiate(menuPrefab, Managers.Canvas);
        }
        else
        {
            Destroy(menu);
        }
    }
}
