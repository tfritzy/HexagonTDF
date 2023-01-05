using UnityEngine.UIElements;

public class ActionDrawer : Drawer
{
    public ActionDrawer(VisualElement root) : base(root)
    {
        Button buildMenuButton = root.Q<Button>("OpenBuildDrawer");
        buildMenuButton.clicked += OpenBuildMode;

        Button inventory = root.Q<Button>("OpenInventory");
        inventory.clicked += OpenInventory;
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