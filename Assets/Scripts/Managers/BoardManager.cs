using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public HexagonMono[,] Hexagons;
    public string ActiveMapName;
    public bool RegenerateMap;

    void Awake()
    {
        SpawnMap();
    }

    void Update()
    {
        if (RegenerateMap)
        {
            CleanupMap();
            SpawnMap();
            RegenerateMap = false;
        }
    }

    private void SpawnMap()
    {
        OverworldSegment segment;
        if (GameState.SelectedSegment == null)
        {
            segment = OverworldTerrainGenerator.GenerateSingleSegment(25, 35, UnityEngine.Random.Range(0, 10));
        }
        else
        {
            segment = GameState.SelectedSegment;
        }

        SpawnHexagons(segment);
        SetupMapHeight(segment);
    }

    private void CleanupMap()
    {
        foreach (HexagonMono hexagon in this.Hexagons)
        {
            Destroy(hexagon?.gameObject);
        }
    }

    HashSet<Biome> flatBiomes = new HashSet<Biome> { Biome.Water, Biome.Sand, Biome.Grassland, Biome.Snow, Biome.Forrest };
    private void SetupMapHeight(OverworldSegment map)
    {
        OpenSimplexNoise heightNoise = new OpenSimplexNoise(map.Index);
        for (int y = 0; y < this.Hexagons.GetLength(1); y++)
        {
            for (int x = 0; x < this.Hexagons.GetLength(0); x++)
            {
                Vector3 pos = this.Hexagons[x, y].transform.position;
                pos.y = 0;
                this.Hexagons[x, y].transform.position = pos;

                if (flatBiomes.Contains(this.Hexagons[x, y].Biome))
                {
                    continue;
                }

                double heightVal = heightNoise.Evaluate(x / 5.0f, y / 5.0f, 1) + 1;
                int hieghtModifier = 0;
                if (heightVal > 1.35f) hieghtModifier += 1;
                if (heightVal > 1.6f) hieghtModifier += 1;
                this.Hexagons[x, y].transform.position += hieghtModifier * Vector3.up;
            }
        }
    }

    private void SpawnHexagons(OverworldSegment segment)
    {
        this.Hexagons = new HexagonMono[segment.Width, segment.Height];

        for (int y = 0; y < segment.Height; y++)
        {
            for (int x = 0; x < segment.Width; x++)
            {
                if (segment.GetPoint(x, y) == null)
                {
                    continue;
                }

                BuildHexagon(segment.GetPoint(x, y), x, y, segment.Index);
            }
        }
    }

    private bool isValidPath(List<Vector2Int> path, Vector2Int expectedStart, Vector2Int expectedEnd)
    {
        return path?.Count > 0 && path[0] == expectedStart && path.Last() == expectedEnd;
    }

    private void BuildHexagon(OverworldMapPoint point, int x, int y, int segmentIndex)
    {
        Vector3 position = Helpers.ToWorldPosition(x, y);
        position.y = point.Height;
        GameObject go = Instantiate(Prefabs.Hexagons[point.Biome], position, new Quaternion(), this.transform);
        HexagonMono hexagonScript = go.GetComponent<HexagonMono>();
        hexagonScript.SetType(Prefabs.GetHexagonScript(point.Biome));
        this.Hexagons[x, y] = go.GetComponent<HexagonMono>();
        this.Hexagons[x, y].GridPosition = new Vector2Int(x, y);
        this.Hexagons[x, y].MaybeSpawnObstacle(segmentIndex);
    }

    public HexagonMono GetHex(Vector2Int pos)
    {
        if (Helpers.IsInBounds(pos, this.Hexagons.GetLength(0)) == false)
        {
            return null;
        }

        return Hexagons[pos.x, pos.y];
    }
}
