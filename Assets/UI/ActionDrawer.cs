using UnityEngine.UIElements;

public class ActionDrawer : Drawer
{
    public ActionDrawer()
    {
        Button buildMenuButton = new Button();
        buildMenuButton.AddToClassList("mode-switch-button");
        buildMenuButton.clicked += OpenBuildMode;
        this.Add(buildMenuButton);

        Button inventory = new Button();
        inventory.AddToClassList("mode-switch-button");
        inventory.clicked += OpenInventory;
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