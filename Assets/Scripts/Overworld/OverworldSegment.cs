using System.Collections.Generic;
using UnityEngine;

public class OverworldSegment
{
    public Hexagon[,] Points;
    public List<string> FortressIds;
    public Dictionary<string, Vector2Int> FortressPositions;
    public Dictionary<string, Alliance> FortressAlliances;
    public Dictionary<Alliance, OverworldTerritory> Territories;
    public int Index;

    public int Width => Points.GetLength(0);
    public int Height => Points.GetLength(1);

    public Hexagon GetPoint(Vector2Int pos)
    {
        return GetPoint(pos.x, pos.y);
    }

    public Hexagon GetPoint(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
        {
            return null;
        }

        return Points[x, y];
    }
}