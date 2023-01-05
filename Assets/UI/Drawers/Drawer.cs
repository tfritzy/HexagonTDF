using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Drawer : UIPage
{
    public Drawer()
    {
        this.AddToClassList("drawer");
        this.style.backgroundColor = UIColors.Dark.PanelBackground;
        this.SetBorderColor(UIColors.Dark.PanelOutline);
    }
}
