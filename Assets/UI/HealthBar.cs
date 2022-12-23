using UnityEngine;
using UnityEngine.UIElements;

public class HealthBar : UIHoverer
{
    public override Hoverer Type => Hoverer.HealthBar;
    public override Vector3 Offset => _offset;
    private Vector3 _offset = new Vector3(-.5f, -2);

    private Label label;

    public HealthBar()
    {
        this.label = new Label();
        label.AddToClassList("small-outlined-text");
        this.Add(label);
    }

    public void Update(int health)
    {
        this.label.text = health.ToString();
    }
}