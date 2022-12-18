using UnityEngine;
using UnityEngine.UIElements;

public abstract class UIHoverer : VisualElement
{
    public abstract Hoverer Type { get; }
    private Transform Target;

    private static Vector3 Down = new Vector3(0, 1, 0);
    private static Vector3 Left = new Vector3(-1, 0);

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
    }

    public void Update()
    {
        if (Target == null)
        {
            Debug.LogWarning("Update called on uiHover with no target.");
            return;
        }

        Vector2 newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(
            this.panel,
            Target.position,
            Managers.Camera);
        this.transform.position = ((Vector3)newPosition) +
            Left * (this.layout.width / 2) +
            Down * (this.layout.height / 2);

        this.MarkDirtyRepaint();
    }
}