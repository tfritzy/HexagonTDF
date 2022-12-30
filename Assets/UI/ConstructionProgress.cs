using UnityEngine;
using UnityEngine.UIElements;

public class ConstructionProgress : UIHoverer
{
    public override Hoverer Type => Hoverer.ConstructionProgress;
    public override Vector3 Offset => _offset;
    private Vector3 _offset = new Vector3(-.5f, -7);
    private VisualElement Inner;

    private Label label;

    public ConstructionProgress()
    {
        this.AddToClassList("construction-bar-outline");

        VisualElement inner = new VisualElement();
        inner.AddToClassList("construction-bar-inner");
        this.Add(inner);
        this.Inner = inner;

        this.Inner.style.width = new StyleLength(new Length(0f, LengthUnit.Percent));
    }

    public void Update(float progress)
    {
        // Because of the border radius on inner, it looks weird at small percent values.
        progress = Mathf.Max(10f, progress);

        this.Inner.style.width = new StyleLength(new Length(progress, LengthUnit.Percent));
        this.style.display = DisplayStyle.Flex;
    }
}