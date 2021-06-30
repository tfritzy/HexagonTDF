using UnityEngine;

public class PredGridPoint
{
    public Vector2Int Position;
    public int Distance;

    public PredGridPoint(Vector2Int position, int distance)
    {
        this.Position = position;
        this.Distance = distance;
    }

    public PredGridPoint()
    {
        this.Position = Constants.MaxVector2Int;
        this.Distance = 0;
    }
}