using UnityEngine.UIElements;

public abstract class UIPage
{
    protected VisualElement Root;

    public UIPage(VisualElement root)
    {
        this.Root = root;
        InitBackButton();
    }

    public void Show()
    {
        Root.style.display = DisplayStyle.Flex;
    }

    public void Hide()
    {
        Root.style.display = DisplayStyle.None;
    }

    private void InitBackButton()
    {
        Button backButton = this.Root.Q<Button>("Back");
        if (backButton != null)
        {
            backButton.clicked += Managers.UI.Back;
        }
    }
}