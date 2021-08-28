using UnityEngine;

public abstract class ObstacleHexagon : Hexagon
{
    public abstract float ObstacleChance { get; }
    public abstract GameObject GetObstacle();
    public abstract float SizeVarience { get; }
    private bool hasObstacle;
    public override bool IsBuildable => !hasObstacle;
    public override bool IsWalkable => !hasObstacle;

    public void GenerateObstacle(Transform hex, Vector2Int hexGridPos)
    {
        System.Random random = new System.Random(GameState.SelectedSegment.Coordinates.GetHashCode() * 786433 * hexGridPos.GetHashCode() * 3145739);
        if (random.NextDouble() <= ObstacleChance)
        {
            hasObstacle = true;
            GameObject obstacle = GameObject.Instantiate(GetObstacle(), hex.transform.position, new Quaternion(), hex);
            float size = Random.Range(1 - SizeVarience, 1 + SizeVarience);
            obstacle.transform.localScale = obstacle.transform.localScale * size;
            obstacle.transform.rotation = Quaternion.Euler(Vector3.up * Random.Range(0, 360));
        }
    }
}