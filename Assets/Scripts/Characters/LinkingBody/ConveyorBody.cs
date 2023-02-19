using System.Linq;
using UnityEngine;

public class ConveyorBody : LinkingBody
{
    public GameObject CurvedBody;
    public GameObject StraightBody;

    private float AngleMiddlepoint(float deg1, float deg2)
    {
        float rad1 = deg1 * Mathf.Deg2Rad;
        float rad2 = deg2 * Mathf.Deg2Rad;
        float avg_rad = (rad1 + rad2) / 2;
        float avg_deg = avg_rad * Mathf.Rad2Deg;
        return NormalizeAngle(avg_deg);
    }

    private float NormalizeAngle(float angle)
    {
        while (angle < 0)
        {
            angle += 360f;
        }

        while (angle >= 360)
        {
            angle -= 360f;
        }

        return angle;
    }

    protected override void SetupBody()
    {
        Debug.Log("Setup body");
        HexSide? inputSide = null;
        if (this.Owner.ConveyorCell?.InputBelts != null && this.Owner.ConveyorCell?.InputBelts?.Values.Count > 0)
        {
            inputSide = this.Owner.ConveyorCell?.InputBelts.Values.ToArray()[0].Side;
        }
        HexSide? outputSide = this.Owner.ConveyorCell.OutputBelt?.Side;
        Debug.Log($"Sides: {inputSide}, {outputSide}");

        if (inputSide != null && outputSide != null)
        {
            float inputAngle = (int)inputSide * -60;
            float outputAngle = (int)outputSide * -60;
            float shortestDelta = Mathf.DeltaAngle(inputAngle, outputAngle);
            float midPoint = inputAngle + shortestDelta / 2;
            shortestDelta = Mathf.Abs(shortestDelta);

            if (shortestDelta > 119 && shortestDelta < 121)
            {
                // Curved case.
                CurvedBody.SetActive(true);
                StraightBody.SetActive(false);

                int medianSide = (int)inputSide + 1;
                CurvedBody.transform.rotation = Quaternion.AngleAxis(midPoint + 240, Vector3.up);
            }
            else if (shortestDelta >= 179)
            {
                // Straight case
                CurvedBody.SetActive(false);
                StraightBody.SetActive(true);

                StraightBody.transform.rotation = Quaternion.AngleAxis(inputAngle, Vector3.up);
            }
        }
        else
        {
            CurvedBody.SetActive(false);
            StraightBody.SetActive(true);

            if (inputSide != null)
            {
                StraightBody.transform.rotation = Quaternion.AngleAxis(((int)inputSide) * -60, Vector3.up);
            }
            else if (outputSide != null)
            {
                StraightBody.transform.rotation = Quaternion.AngleAxis(((int)outputSide) * -60, Vector3.up);
            }
        }
    }
}