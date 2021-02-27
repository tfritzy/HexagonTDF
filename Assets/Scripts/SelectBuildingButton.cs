using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectBuildingButton : MonoBehaviour
{
    public Building Building;
    private Image icon;
    private Sprite originalIcon;
    void Start()
    {
        icon = this.transform.Find("Image").GetComponent<Image>();
        originalIcon = icon.sprite;
    }

    public void Click()
    {
        if (Managers.Builder.SelectedBuilding == null)
        {
            Managers.Builder.SelectedBuilding = Building;
            icon.sprite = Prefabs.UIIcons[UIIconType.Exit];
        }
        else
        {
            Managers.Builder.SelectedBuilding = null;
            icon.sprite = originalIcon;
        }
    }
}
