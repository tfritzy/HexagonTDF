using UnityEngine;
using UnityEngine.UIElements;

public class HarvestProgress : UIHoverer
{
    public override Hoverer Type => Hoverer.HarvestProgress;
    public override Vector3 Offset => _offset;
    private Vector3 _offset = new Vector3(-.5f, -8);
    private VisualElement Inner;

    private Label label;

    public HarvestProgress()
    {
        this.AddToClassList("harvest-progress-bar-outline");

        VisualElement inner = new VisualElement();
        inner.AddToClassList("harvest-progress-bar-inner");
        this.Add(inner);
        this.Inner = inner;

        this.Inner.style.width = new StyleLength(new Length(0f, LengthUnit.Percent));
    }

    public void Update(float progress)
    {
        // Because of the border radius on inner, it looks weird at small percent values.
        progress = Mathf.Max(5f, progress);
        progress = Mathf.Min(100f, progress);

        this.Inner.style.width = new StyleLength(new Length(progress, LengthUnit.Percent));
        this.style.display = DisplayStyle.Flex;
    }
}