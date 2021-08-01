using UnityEngine;

public class Waypoint
{
    public Vector2Int StartPos;
    public Vector2Int EndPos;
    public Vector3 Offset;

    public Vector3 WorldspaceStartPos => Helpers.ToWorldPosition(StartPos) + Offset;
    public Vector3 WorldspaceEndPos => Helpers.ToWorldPosition(EndPos) + Offset;

    /// <summary>
    /// Whether or not this waypoint can be calculated from the map's pred grid.
    /// This was made for the situation where an enemy is walking off a boat to a dock.
    /// In this case the boat is a temporary structure that is traversable, but not on
    /// the map data, so while walking this waypoint, recalculate requests should be ignored.
    /// </summary>
    public bool IsRecalculable;

    public Waypoint(Vector2Int startPos, Vector2Int endPos, Vector3 offset)
    {
        this.StartPos = startPos;
        this.EndPos = endPos;
        this.Offset = offset;
    }
}