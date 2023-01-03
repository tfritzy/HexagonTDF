using UnityEngine;
using UnityEngine.UIElements;

public abstract class UIHoverer : VisualElement
{
    public abstract Hoverer Type { get; }
    // Offset position around target in terms of percent of element size.
    public abstract Vector3 Offset { get; }
    private Transform Target;
    private int UpdateAfter1Frame = 0;

    private static Vector3 Down = new Vector3(0, 1);
    private static Vector3 Right = new Vector3(1, 0);
    private static Vector3 Up = new Vector3(0, 0, 1);

    public UIHoverer()
    {
        this.style.position = new StyleEnum<Position>(Position.Absolute);
        this.style.visibility = new StyleEnum<Visibility>(Visibility.Hidden);
    }

    public void Show()
    {
        this.style.display = DisplayStyle.Flex;
    }

    public void Hide()
    {
        this.style.display = DisplayStyle.None;
    }

    public void SetTarget(Transform target)
    {
        this.Target = target;
        Update();
    }

    public virtual void Update()
    {
        if (Target == null)
        {
            Managers.UI.HideHoverer(this);
            return;
        }

        Vector2 newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(
                this.panel,
                Target.position,
                Managers.Camera);
        this.transform.position = ((Vector3)newPosition) +
            Right * (this.layout.width * Offset.x) +
            Down * (this.layout.height * Offset.y);

        if (UpdateAfter1Frame == 2)
        {
            this.style.visibility = new StyleEnum<Visibility>(Visibility.Visible);
        }

        if (UpdateAfter1Frame < 3)
        {
            UpdateAfter1Frame += 1;
        }
    }
}