using UnityEngine.UIElements;

public class ActionDrawer : Drawer
{
    public ActionDrawer()
    {
        Button buildMenuButton = new LargeSquareButton();
        buildMenuButton.clicked += OpenBuildMode;
        buildMenuButton.style.backgroundImage = new StyleBackground(Icons.GetUiIcon(UIIconType.Hammer));
        this.Add(buildMenuButton);

        Button inventory = new LargeSquareButton();
        inventory.clicked += OpenInventory;
        inventory.style.backgroundImage = new StyleBackground(Icons.GetUiIcon(UIIconType.Inventory));
        this.Add(inventory);
    }

    private void OpenBuildMode()
    {
        Managers.InputManager.OpenBuildMode();
        Managers.UI.ShowPage(Page.BuildDrawer);
    }

    private void OpenInventory()
    {
        Managers.UI.ShowPage(Page.PlayerInventory);
    }
}