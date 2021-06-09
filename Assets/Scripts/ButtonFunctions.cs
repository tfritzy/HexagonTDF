using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonFunctions : MonoBehaviour
{
    public Building Building;
    private Image icon;
    private Sprite originalIcon;
    private bool thinksAmActiveButton;

    void Start()
    {
        this.thinksAmActiveButton = false;
        this.icon = this.transform.Find("Icon").GetComponent<Image>();
        this.originalIcon = this.icon.sprite;
    }

    void Update()
    {
        if (thinksAmActiveButton && Managers.Builder.SelectedBuilding != Building)
        {
            this.icon.sprite = this.originalIcon;
            thinksAmActiveButton = false;
        }
    }

    public void SelectBuilding()
    {
        if (Managers.Builder.SelectedBuilding == Building && thinksAmActiveButton)
        {
            Managers.Builder.AcceptConstructBuilding();
        }
        else
        {
            Managers.Builder.SetSelectedBuilding(this, Building);
            this.icon.sprite = Prefabs.UIIcons[UIIconType.Accept];
            this.thinksAmActiveButton = true;
        }
    }

    public void AcceptBuildingConstruction()
    {
        Managers.Builder.AcceptConstructBuilding();
        this.thinksAmActiveButton = false;
    }
}
