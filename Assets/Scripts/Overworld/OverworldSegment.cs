using System.Collections.Generic;
using UnityEngine;

public class OverworldSegment
{
    public OverworldMapPoint[,] Points;
    public List<string> FortressIds;
    public Dictionary<string, Vector2Int> FortressPositions;
    public Dictionary<string, Alliances> FortressAlliances;
    public Dictionary<Alliances, OverworldTerritory> Territories;
    public int Index;

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