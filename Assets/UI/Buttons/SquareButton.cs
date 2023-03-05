using UnityEngine;
using UnityEngine.UIElements;

public class SquareButton : Button
{
    public SquareButton(System.Action onClick, Sprite icon)
    {
        this.AddToClassList("grid-button");
        this.style.backgroundColor = UIColors.Dark.PrimaryButton;
        this.SetBorderColor(UIColors.Dark.PrimaryButtonOutline);
        this.style.unityBackgroundImageTintColor = UIColors.Dark.PrimaryButtonTint;

        this.clicked += onClick;
        this.style.backgroundImage = new StyleBackground(icon);
    }
}
