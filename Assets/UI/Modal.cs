using UnityEngine;
using UnityEngine.UIElements;

public class Modal : UIPage
{
    public Modal(int width)
    {
        this.style.width = width;
        this.style.backgroundColor = UIColors.Dark.PanelBackground;
        this.SetBorderColor(UIColors.Dark.PanelOutline);
        this.AddToClassList("modal");

        VisualElement header = new VisualElement();
        header.AddToClassList("modal-header");
        this.Add(header);

        Button backButton = new Button();
        backButton.clicked += () => Managers.UI.Back();
        backButton.AddToClassList("modal-x-button");
        backButton.style.backgroundImage = new StyleBackground(Icons.GetUiIcon(UIIconType.X));
        backButton.style.unityBackgroundImageTintColor = UIColors.Dark.BrightRed;
        backButton.SetBorderColor(new Color(0, 0, 0, 0));
        header.Add(backButton);
    }
}