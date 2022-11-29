using UnityEngine.UIElements;

public class ActionDrawer : Drawer
{
    public ActionDrawer(VisualElement root) : base (root)
    {
        Button buildMenuButton = root.Q<Button>("OpenBuildDrawer");
        buildMenuButton.clicked += OpenBuildMode;
    }

    private void OpenBuildMode()
    {
        Managers.InputManager.OpenBuildMode();
        Managers.UI.ShowPage(Page.BuildDrawer);
    }
}