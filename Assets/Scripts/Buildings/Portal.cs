using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : Building
{
    public override BuildingType Type => BuildingType.Portal;
    public override Alliances Alliance => Alliances.Illigons;
    public override Alliances Enemies => Alliances.Player;

    public GameObject Dot;
    public GameObject Enemy;

    private List<Vector2Int> pathToSource;

    protected override void Setup()
    {
        dots = new List<GameObject>();
        RecalculatePath();
        base.Setup();
    }

    private List<GameObject> dots;
    public void RecalculatePath()
    {
        foreach (GameObject dot in dots)
        {
            Destroy(dot);
        }

        pathToSource = Helpers.FindPath(Managers.Map.Hexagons, Position, Managers.Map.Source.Position);

        if (pathToSource == null)
        {
            throw new System.NullReferenceException($"Portal was unable to find path to source.");
        }

        foreach (Vector2Int position in pathToSource)
        {
            dots.Add(Instantiate(Dot, Hexagon.ToWorldPosition(position) + Vector3.up, new Quaternion(), null));
        }
    }

    private float lastEnemyTime;
    protected override void UpdateLoop()
    {
        if (Time.time > lastEnemyTime + 5f)
        {
            GameObject enemy = Instantiate(Enemy, this.transform.position, new Quaternion(), null);
            enemy.GetComponent<Enemy>().SetPath(pathToSource);
            lastEnemyTime = Time.time;
        }
    }
}
