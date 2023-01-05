using UnityEngine.UIElements;

public class SquareButton : Button
{
    public SquareButton()
    {
        this.AddToClassList("grid-button");
        this.style.backgroundColor = UIColors.Dark.PrimaryButton;
        this.SetBorderColor(UIColors.Dark.PrimaryButtonOutline);
    }
}