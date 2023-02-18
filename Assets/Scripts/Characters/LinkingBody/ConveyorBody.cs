using UnityEngine;

public class ConveyorBody : LinkingBody
{
    protected override void SetupBody()
    {
        Debug.Log("Setup body");
        HexSide? outputSide = this.Owner.ConveyorCell.OutputBelt?.Side;

        if (outputSide != null)
        {
            this.transform.rotation = Quaternion.AngleAxis(((int)outputSide) * 30, Vector3.up);
        }
    }
}