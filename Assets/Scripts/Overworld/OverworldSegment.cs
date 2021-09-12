using System.Collections.Generic;
using UnityEngine;

public class OverworldSegment
{
    public OverworldMapPoint[,] Points;
    public Texture2D Texture;
    public List<Vector2Int> Fortresses;
    public Vector2Int Coordinates;

    public int Width => Points.GetLength(0);
    public int Height => Points.GetLength(1);

    public OverworldMapPoint GetPoint(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
        {
            return null;
        }

        return Points[x, y];
    }
}