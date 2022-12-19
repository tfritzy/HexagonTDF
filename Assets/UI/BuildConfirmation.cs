using System;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildConfirmation : UIHoverer
{
    public override Hoverer Type => Hoverer.BuildConfirmation;
    public override Vector2 Offset => _offset;
    private Vector2 _offset = new Vector2(-.5f, .5f);

    private Action onConfirm;
    private Action onCancel;
    public BuildConfirmation()
    {
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