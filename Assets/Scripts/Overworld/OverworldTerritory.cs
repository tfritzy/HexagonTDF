using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldTerritory
{
    private List<Vector2Int> points;
    public List<Vector2Int> Points
    {
        get { return points; }
        set
        {
            points = value;
            CalculateBounds();
        }
    }

    public HashSet<Vector2Int> Edges { get; set; }
    public List<Vector2Int> Outline { get; private set; }
    public Vector2Int HighBounds { get; private set; }
    public Vector2Int LowBounds { get; private set; }
    public Vector2 Center { get; private set; }
    public Vector2Int Size { get; private set; }
    public Texture2D Texture { get; private set; }

    private void CalculateBounds()
    {
        Vector2Int highBounds = Vector2Int.zero;
        Vector2Int lowBounds = Constants.MaxVector2Int;
        foreach (Vector2Int point in points)
        {
            if (point.x > highBounds.x)
            {
                highBounds.x = point.x;
            }

            if (point.y > highBounds.y)
            {
                highBounds.y = point.y;
            }

            if (point.x < lowBounds.x)
            {
                lowBounds.x = point.x;
            }

            if (point.y < lowBounds.y)
            {
                lowBounds.y = point.y;
            }
        }

        this.HighBounds = highBounds;
        this.LowBounds = lowBounds;

        this.Size = new Vector2Int(
            HighBounds.x - LowBounds.x + 1,
            HighBounds.y - LowBounds.y + 1
        );

        this.Center = new Vector2(
            LowBounds.x + (float)Size.x / 2,
            LowBounds.y + (float)Size.y / 2
        );
    }
}