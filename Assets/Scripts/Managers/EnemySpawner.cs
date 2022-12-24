using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject Enemy;

    private float lastSpawnTime;
    void Update()
    {
        if (Time.time > lastSpawnTime + 1f)
        {
            Vector2Int spawnPos = Managers.Board.Navigation.Terminations[Random.Range(0, Managers.Board.Navigation.Terminations.Count)];
            var enemyGo = Instantiate(Enemy, Helpers.ToWorldPosition(spawnPos), new Quaternion());
            lastSpawnTime = Time.time;
        }
    }

    // void Start()
    // {
    //     foreach (Vector2Int termination in Managers.Board.Navigation.Terminations)
    //     {
    //         Instantiate(Enemy, Helpers.ToWorldPosition(termination), new Quaternion());
    //     }
    // }
}