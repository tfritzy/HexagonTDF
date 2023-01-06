using System.Collections.Generic;
using UnityEngine;

public enum UIIconType
{
    Hammer,
    Inventory,
    X
};

public static class Icons
{
    private static Dictionary<UIIconType, Sprite> _uiIcons = new Dictionary<UIIconType, Sprite>();
    public static Sprite GetUiIcon(UIIconType icon)
    {
        if (!_uiIcons.ContainsKey(icon))
        {
            _uiIcons[icon] = Resources.Load<Sprite>($"Icons/UI/{icon}");
        }

        return _uiIcons[icon];
    }

    private static Dictionary<BuildingType, Sprite> _buildingIcons = new Dictionary<BuildingType, Sprite>();
    public static Sprite GetBuildingIcon(BuildingType building)
    {
        if (!_buildingIcons.ContainsKey(building))
        {
            _buildingIcons[building] = Resources.Load<Sprite>($"Icons/Buildings/{building}");
        }

        return _buildingIcons[building];
    }

}