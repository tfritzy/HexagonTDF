using System;
using UnityEngine.UIElements;

public class BuildConfirmation : UIHoverer
{
    public override Hoverer Type => Hoverer.BuildConfirmation;
    private Action onConfirm;
    private Action onCancel;
    public BuildConfirmation()
    {
        this.style.position = new StyleEnum<Position>(Position.Absolute);

        var confirmButton = new Button();
        confirmButton.AddToClassList("wide-button");
        confirmButton.clicked += Confirm;
        confirmButton.text = "Build";
        this.Add(confirmButton);

        var cancelButton = new Button();
        cancelButton.AddToClassList("wide-button");
        cancelButton.clicked += Cancel;
        cancelButton.text = "Cancel";
        this.Add(cancelButton);
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