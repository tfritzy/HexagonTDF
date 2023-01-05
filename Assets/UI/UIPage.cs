using UnityEngine.UIElements;

public abstract class UIPage : VisualElement
{
    public void Show()
    {
        this.style.display = DisplayStyle.Flex;
    }

    public void Hide()
    {
        this.style.display = DisplayStyle.None;
    }

    public virtual void Update() { }
}