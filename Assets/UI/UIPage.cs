using UnityEngine.UIElements;

public abstract class UIPage
{
    protected VisualElement Root;

    public UIPage(VisualElement root)
    {
        this.Root = root;
    }

    public void Show()
    {
        Root.style.display = DisplayStyle.Flex;
    }

    public void Hide()
    {
        Root.style.display = DisplayStyle.None;
    }

    public virtual void Update() { }
}