using UnityEngine;

public abstract class ObstacleHexagon : Hexagon
{
    public abstract float ObstacleChance { get; }
    public abstract GameObject GetObstacle();
    private bool hasObstacle;
    public override bool IsBuildable => !hasObstacle;
    public override bool IsWalkable => !hasObstacle;

    public void GenerateObstacle(Transform hex, Vector2Int hexGridPos)
    {
        System.Random random = new System.Random(GameState.SelectedSegment.Coordinates.GetHashCode() * 786433 * hexGridPos.GetHashCode() * 3145739);
        if (random.NextDouble() <= ObstacleChance)
        {
            hasObstacle = true;
            GameObject.Instantiate(GetObstacle(), hex.transform.position, new Quaternion(), hex);
        }
    }
}