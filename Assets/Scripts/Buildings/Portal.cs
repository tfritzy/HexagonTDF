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
    public override ResourceTransaction BuildCost => null;
    public float Power;

    private List<Vector2Int> pathToSource;
    private float levelStartTime;
    private readonly List<float> powerGainRate30SecondInterval = new List<float>()
    {
        .2f,
        .2f,
        .7f,
        1.6f,
        3.3f,
        8.2f,
        15.5f,
        24.4f,
        33.3f,
        42.2f,
        51.1f
    };

    protected override void Setup()
    {
        dots = new List<GameObject>();
        RecalculatePath();
        base.Setup();
        levelStartTime = Time.time;
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
    private float lastPowerTime;
    protected override void UpdateLoop()
    {
        if (Power > 3)
        {
            GameObject enemy = Instantiate(Enemy, this.transform.position, new Quaternion(), null);
            enemy.GetComponent<Enemy>().SetPath(pathToSource);
            Power -= 3;
            lastEnemyTime = Time.time;
        }

        if (Time.time > lastPowerTime + 1f)
        {
            Power += powerGainRate30SecondInterval[(int)((Time.time - levelStartTime) / 30)];
            lastPowerTime = Time.time;
        }
    }
}
