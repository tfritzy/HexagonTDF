using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CraftingMenu : VisualElement
{
    private const int hexWidth = 60;
    private const float HEX_R = Constants.HEXAGON_R * hexWidth;
    private const float HEX_r = Constants.HEXAGON_r * hexWidth;
    const float horizontalDistBetweenHex = (HEX_R + HEX_r / 2) * Constants.HEXAGON_R;
    const float verticalDistBetweenHex = (HEX_r * 2);
    const float padding = 1.1f;

    private static HashSet<Vector2Int> KeptHexes = new HashSet<Vector2Int>
    {
        new Vector2Int(1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(1, 1),
        new Vector2Int(2, 1),
        new Vector2Int(0, 2),
        new Vector2Int(1, 2),
        new Vector2Int(2, 2),
    };

    public CraftingMenu()
    {
        var container = new VisualElement();
        this.Add(container);
        container.style.alignSelf = new StyleEnum<Align>(Align.Center);
        container.style.width = HEX_R * 5 * padding;
        container.style.height = HEX_r * 8 * padding;

        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if (!KeptHexes.Contains(new Vector2Int(x, y)))
                {
                    continue;
                }

                Vector2 pos = Helpers.ToWorldPosition(x, y);

                VisualElement hex = new VisualElement();
                hex.AddToClassList("crafting-menu-hex");

                float xF = x * horizontalDistBetweenHex;
                float zF =
                    y * verticalDistBetweenHex +
                    (x % 2 == 1 ? HEX_r : 0);
                xF *= 1.1f;
                zF *= 1.1f;

                hex.style.position = new StyleEnum<Position>(Position.Absolute);
                hex.style.width = HEX_R * 2;
                hex.style.height = HEX_r * 2;
                hex.style.left = new StyleLength(xF);
                hex.style.top = new StyleLength(zF);
                hex.style.unityBackgroundImageTintColor = UIColors.Dark.InventorySlotBackground;
                container.Add(hex);
            }
        }
    }
}