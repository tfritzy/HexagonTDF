using UnityEngine;
using UnityEngine.UIElements;

public class UIHoverer
{
    protected VisualElement Root;
    private Transform Target;

    private static Vector3 Down = new Vector3(0, 1, 0);
    private static Vector3 Left = new Vector3(-1, 0);

    public UIHoverer(VisualElement root)
    {
        this.Root = root;
        Hide();
    }

    public void Show()
    {
        Root.style.display = DisplayStyle.Flex;
    }

    public void Hide()
    {
        Root.style.display = DisplayStyle.None;
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
            Root.panel,
            Target.position,
            Managers.Camera);
        Root.transform.position = ((Vector3)newPosition) +
            Left * (Root.layout.width / 2) +
            Down * (Root.layout.height + Root.layout.height / 5);
    }
}