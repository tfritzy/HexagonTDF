using UnityEngine;
using UnityEngine.UIElements;

public class HealthBar : UIHoverer
{
    public override Hoverer Type => Hoverer.HealthBar;
    public override Vector3 Offset => _offset;
    private Vector3 _offset = new Vector3(-.5f, -10);
    private VisualElement Inner;

    private Label label;

    public HealthBar()
    {
        this.AddToClassList("healthbar-outline");

        VisualElement inner = new VisualElement();
        inner.AddToClassList("healthbar-inner");
        this.Add(inner);
        this.Inner = inner;
    }

    public void Update(int health, int maxHealth)
    {
        if (health != maxHealth)
        {
            float percent = (float)health / (float)maxHealth;
            percent *= 100f;
            this.Inner.style.width = new StyleLength(new Length(percent, LengthUnit.Percent));
            this.style.display = DisplayStyle.Flex;
        }
        else
        {
            this.style.display = DisplayStyle.None;
        }
    }
}