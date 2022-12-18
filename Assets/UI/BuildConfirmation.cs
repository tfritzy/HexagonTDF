using System;
using UnityEngine.UIElements;

public class BuildConfirmation : UIHoverer
{
    public override Hoverer Type => Hoverer.BuildConfirmation;
    private Action onConfirm;
    private Action onCancel;
    private Button ConfirmButton { get; set; }
    private Button CancelButton { get; set; }

    public BuildConfirmation(VisualElement root) : base(root)
    {
        root.style.position = new StyleEnum<Position>(Position.Absolute);

        ConfirmButton = new Button();
        ConfirmButton.AddToClassList("wide-button");
        ConfirmButton.clicked += Confirm;
        ConfirmButton.text = "Build";
        ConfirmButton.style.display = DisplayStyle.Flex;
        root.Add(ConfirmButton);

        CancelButton = new Button();
        CancelButton.AddToClassList("wide-button");
        CancelButton.clicked += Cancel;
        CancelButton.text = "Cancel";
        CancelButton.style.display = DisplayStyle.Flex;
        root.Add(CancelButton);

        this.Show();
    }

    public void Init(Action onConfirm, Action onCancel)
    {
        this.onConfirm = onConfirm;
        this.onCancel = onCancel;
    }

    private void Confirm()
    {
        this.onConfirm();
    }

    private void Cancel()
    {
        this.onCancel();
    }
}