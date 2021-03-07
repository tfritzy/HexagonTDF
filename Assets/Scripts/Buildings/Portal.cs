using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : Building
{
    public override BuildingType Type => BuildingType.Portal;
    public GameObject Dot;
    public GameObject Enemy;

    private List<Vector2Int> pathToSource;

    protected override void Setup()
    {
        pathToSource = Helpers.FindPath(Managers.BoardManager.Hexagons, Position, Managers.BoardManager.Source.Position);
        foreach (Vector2Int position in pathToSource)
        {
            Instantiate(Dot, Hexagon.ToWorldPosition(position) + Vector3.up, new Quaternion(), null);
            Debug.Log(position);
        }

        base.Setup();
    }

    private float lastEnemyTime;
    protected override void UpdateLoop()
    {
        if (Time.time > lastEnemyTime + 5f)
        {
            GameObject enemy = Instantiate(Enemy, this.transform.position, new Quaternion(), null);
            enemy.GetComponent<Enemy>().Setup(pathToSource);
            lastEnemyTime = Time.time;
        }
    }
}
