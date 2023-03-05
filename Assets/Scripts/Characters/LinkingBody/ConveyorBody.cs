using System.Linq;
using UnityEngine;

public class ConveyorBody : MonoBehaviour
{
    public GameObject CurvedBody;
    public GameObject StraightBody;

    public Transform[] CurvePoints;
    public Transform[] StraightPoints;

    private Transform[] reverseCurvePoints;
    private Transform[] reverseStraightPoints;

    public Transform[] ActivePoints =>
        StraightBody.activeInHierarchy ?
            (isReverseCase ? reverseStraightPoints : StraightPoints) :
            (isReverseCase ? reverseCurvePoints : CurvePoints);
    public float TotalLength => StraightBody.activeInHierarchy ? straightPathLength : curvedPathLength;
    private static float curvedPathLength;
    private static float straightPathLength;

    private bool isReverseCase;

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

    private void InitPointMetadata()
    {
        this.reverseCurvePoints = new Transform[this.CurvePoints.Length];
        this.CurvePoints.CopyTo(reverseCurvePoints, 0);
        this.reverseCurvePoints = this.reverseCurvePoints.Reverse().ToArray();
        this.reverseStraightPoints = new Transform[this.StraightPoints.Length];
        this.StraightPoints.CopyTo(reverseStraightPoints, 0);
        this.reverseStraightPoints = this.reverseStraightPoints.Reverse().ToArray();

        if (curvedPathLength == 0)
        {
            for (int i = 1; i < this.CurvePoints.Length; i++)
            {
                curvedPathLength += (this.CurvePoints[i].transform.position - this.CurvePoints[i - 1].transform.position).magnitude;
            }

            for (int i = 1; i < this.StraightPoints.Length; i++)
            {
                straightPathLength += (this.StraightPoints[i].transform.position - this.StraightPoints[i - 1].transform.position).magnitude;
            }
        }
    }

    public void Setup()
    {
        InitPointMetadata();

        Character owner = this.transform.parent.GetComponent<Character>();
        HexSide? inputSide = owner.ConveyorCell?.ConveyorBelt?.InputSide;
        HexSide? outputSide = owner.ConveyorCell?.ConveyorBelt?.OutputSide;

        Debug.Log($"Setting up body with {inputSide}, {outputSide}");

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
                isReverseCase = midPoint < inputAngle;

                TextureScroll textureScroll = CurvedBody.GetComponentInChildren<TextureScroll>();
                if (textureScroll)
                {
                    textureScroll.direction = isReverseCase ? -1 : 1;
                }
            }
            else if ((shortestDelta < 181 && shortestDelta >= 179))
            {
                // Straight case
                CurvedBody.SetActive(false);
                StraightBody.SetActive(true);

                StraightBody.transform.rotation = Quaternion.AngleAxis(inputAngle, Vector3.up);
                isReverseCase = false;
            }
            else
            {
                Debug.LogWarning($"Conveyor has impossible input/output configuration. angle: {shortestDelta}");
            }
        }
        else
        {
            CurvedBody.SetActive(false);
            StraightBody.SetActive(true);
            isReverseCase = false;

            if (inputSide != null)
            {
                StraightBody.transform.rotation = Quaternion.AngleAxis(((int)inputSide) * -60, Vector3.up);
            }
            else if (outputSide != null)
            {
                StraightBody.transform.rotation = Quaternion.AngleAxis(((int)outputSide) * -60 + 180, Vector3.up);
            }
        }
    }
}