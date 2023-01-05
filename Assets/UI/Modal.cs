using UnityEngine.UIElements;

public class Modal : VisualElement
{
    public Modal(int width)
    {
        this.style.width = width;
        this.style.backgroundColor = UIColors.Dark.PanelBackground;
        this.SetBorderColor(UIColors.Dark.PanelOutline);
        this.AddToClassList("modal");

        Button backButton = new Button();
        backButton.clicked += () => Managers.UI.Back();
        this.Add(backButton);
    }
}