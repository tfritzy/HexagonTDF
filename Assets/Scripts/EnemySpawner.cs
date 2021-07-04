// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;
// using UnityEngine.UI;

// public class EnemySpawner : MonoBehaviour
// {
//     private Color shoreIncomingAttackerColor = ColorExtensions.Create("C3545F", 255);
//     private Color shoreActiveColor = ColorExtensions.Create("414141");
//     private Color shoreInactiveColor = ColorExtensions.Create("434343", 0);

//     private const float WAVE_DURATION_SEC = 30f;
//     private const float DEFAULT_SEC_BETWEEN_SPAWN = 5f;
//     private int _currentWave;
//     public int CurrentWave
//     {
//         get { return _currentWave; }
//         set
//         {
//             _currentWave = value;
//             startWaveCounter.text = _currentWave.ToString();
//         }
//     }
//     private float levelStartTime;
//     private float lastWaveStartTime;
//     private readonly List<float> PowerPerSecondByWave = new List<float>()
//     {
//         0.1f,
//         0.42f,
//         0.924f,
//         2.0328f,
//         4.47216f,
//         9.838752f,
//         21.6452544f,
//         47.61955968f,
//         104.763031296f,
//         230.4786688512f,
//         507.05307147264f,
//     };
//     private GameObject startWaveDialog;
//     private Text startWaveTimer;
//     private Text startWaveCounter;
//     private Dictionary<ResourceType, Text> startWaveResourceBonusTexts;
//     private readonly List<EnemyType> enemies = new List<EnemyType>()
//     {
//         EnemyType.StickGuy,
//         EnemyType.Spellcaster
//     };
//     private readonly List<ResourceType> bonusResourceTypes = new List<ResourceType>()
//     {
//         ResourceType.Gold
//     };
//     private EnemyType currentWaveEnemy;
//     private List<ShoreMono> shores;
//     private const int NumShoresActivatedPerWave = 2;

//     void Start()
//     {
//         startWaveDialog = Managers.Canvas.Find("StartWaveDialog").gameObject;
//         startWaveTimer = startWaveDialog.transform.Find("Content Group").Find("Time").GetComponent<Text>();
//         startWaveCounter = startWaveDialog.transform.Find("Avatar Frame").Find("Mask").Find("WaveCounter").GetComponent<Text>();
//         startWaveResourceBonusTexts = new Dictionary<ResourceType, Text>();
//         Transform resources = startWaveDialog.transform.Find("Content Group").Find("Resources");
//         startWaveResourceBonusTexts[ResourceType.Gold] = resources.Find("Gold").Find("Text").GetComponent<Text>();
//         startWaveDialog.SetActive(false);
//         levelStartTime = Time.time;
//         lastWaveStartTime = Time.time + WAVE_DURATION_SEC * 2;
//         CurrentWave = 1;
//     }

//     public void SetShoreHexes(List<Vector2Int> shoreHex)
//     {
//         this.shores = new List<ShoreMono>();

//         foreach (Vector2Int pos in shoreHex)
//         {
//             shores.Add(Managers.Board.Hexagons[pos.x, pos.y].GetComponent<ShoreMono>());
//         }
//     }

//     public void StartWaveEarly()
//     {
//         foreach (ResourceType type in bonusResourceTypes)
//         {
//             Managers.ResourceStore.Add(type, GetStartEarlyBonus(type));
//         }
//         lastWaveStartTime = Time.time;
//         startWaveDialog.SetActive(false);
//     }

//     private float lastSpawnTime;
//     void Update()
//     {
//         if (Time.time - lastWaveStartTime < 0)
//         {
//             UpdateStartWaveDialog();
//             startWaveTimer.text = $"{(int)(lastWaveStartTime - Time.time)}s";
//             startWaveDialog.SetActive(true);
//             return;
//         }

//         if (Time.time - lastWaveStartTime >= WAVE_DURATION_SEC)
//         {
//             AdvanceWave();
//             return;
//         }

//         startWaveDialog.SetActive(false);
//         if (Time.time > lastSpawnTime + DEFAULT_SEC_BETWEEN_SPAWN)
//         {
//             Vector2Int boatPos = getBoatSpawnPos();
//             ShoreMono shore = this.shores[Random.Range(0, this.shores.Count)];
//             Boat boat = Instantiate(
//                 Prefabs.Enemies[EnemyType.Boat],
//                 Helpers.ToWorldPosition(boatPos),
//                 new Quaternion()).GetComponent<Boat>();
//             boat.SetInitialPos(boatPos);
//             float boatPower = PowerPerSecondByWave[CurrentWave] * DEFAULT_SEC_BETWEEN_SPAWN;
//             for (int i = 0; i < boat.Capacity; i++)
//             {
//                 GameObject enemy = Instantiate(Prefabs.Enemies[enemies[Random.Range(0, enemies.Count)]].gameObject, Vector3.zero, new Quaternion(), boat.transform);
//                 Enemy enemyMono = enemy.GetComponent<Enemy>();
//                 boat.AddPassanger(enemyMono);
//                 enemyMono.SetPower(boatPower / boat.Capacity, 1f);
//             }
//             lastSpawnTime = Time.time;
//         }
//     }

//     private Vector2Int getBoatSpawnPos()
//     {
//         switch (Random.Range(0, 4))
//         {
//             case (0):
//                 return new Vector2Int(0, Random.Range(0, Managers.Board.Map.Height));
//             case (1):
//                 return new Vector2Int(Random.Range(0, Managers.Board.Map.Width), Managers.Board.Map.Height - 1);
//             case (2):
//                 return new Vector2Int(Managers.Board.Map.Width - 1, Random.Range(0, Managers.Board.Map.Height));
//             case (3):
//                 return new Vector2Int(Random.Range(0, Managers.Board.Map.Width), 0);
//             default:
//                 throw new System.Exception("Invalid boat spawn pos logic.");
//         }
//     }

//     private void AdvanceWave()
//     {
//         CurrentWave += 1;
//         lastWaveStartTime = Time.time + WAVE_DURATION_SEC;
//     }

//     private void UpdateStartWaveDialog()
//     {
//         foreach (ResourceType type in bonusResourceTypes)
//         {
//             startWaveResourceBonusTexts[type].text = GetStartEarlyBonus(type).ToString();
//         }
//     }

//     private int GetStartEarlyBonus(ResourceType type)
//     {
//         return (int)(((lastWaveStartTime - Time.time) * PowerPerSecondByWave[CurrentWave]) / Constants.ResourcePowerMap[type]);
//     }
// }