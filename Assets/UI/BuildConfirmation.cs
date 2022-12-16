using System;
using UnityEngine.UIElements;

public class BuildConfirmation : UIHoverer
{
    private Action onConfirm;
    private Action onCancel;
    private Button ConfirmButton { get; set; }
    private Button CancelButton { get; set; }

    public BuildConfirmation(VisualElement root) : base(root)
    {
        ConfirmButton = root.Q<Button>("Confirm");
        CancelButton = root.Q<Button>("Cancel");

        ConfirmButton.clicked += Confirm;
        CancelButton.clicked += Cancel;
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