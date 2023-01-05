using UnityEngine.UIElements;

public class LargeSquareButton : Button
{
    public LargeSquareButton()
    {
        this.AddToClassList("mode-switch-button");
        this.style.backgroundColor = UIColors.Dark.PrimaryButton;
        this.SetBorderColor(UIColors.Dark.PrimaryButtonOutline);
    }
}