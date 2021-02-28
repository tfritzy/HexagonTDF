using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonFunctions : MonoBehaviour
{
    public Building Building;
    private Image icon;
    private Sprite originalIcon;

    void Start()
    {
        icon = this.transform.Find("Image")?.GetComponent<Image>();
        originalIcon = icon?.sprite;
    }

    public void SelectBuilding()
    {
        if (Managers.Builder.SelectedBuilding == null)
        {
            Managers.Builder.SetSelectedBuilding(this, Building);
            icon.sprite = Prefabs.UIIcons[UIIconType.Exit];
        }
        else
        {
            Managers.Builder.SetSelectedBuilding(this, null);
            icon.sprite = originalIcon;
        }
    }

    public void RevertIcon()
    {
        icon.sprite = originalIcon;
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
