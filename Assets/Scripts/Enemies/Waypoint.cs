using UnityEngine;

public class Waypoint
{
    public Vector2Int StartPos;
    public Vector2Int EndPos;

    /// <summary>
    /// Whether or not this waypoint can be calculated from the map's pred grid.
    /// This was made for the situation where an enemy is walking off a boat to a dock.
    /// In this case the boat is a temporary structure that is traversable, but not on
    /// the map data, so while walking this waypoint, recalculate requests should be ignored.
    /// </summary>
    public bool IsRecalculable;

    public Waypoint(Vector2Int startPos, Vector2Int endPos, bool isRecalculable = true)
    {
        this.StartPos = startPos;
        this.EndPos = endPos;
        this.IsRecalculable = isRecalculable;
    }

    public Waypoint(bool isRecalculable = true)
    {
        this.IsRecalculable = isRecalculable;
    }
}