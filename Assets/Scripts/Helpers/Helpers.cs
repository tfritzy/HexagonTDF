using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class Helpers
{
    public static HexagonMono FindHexByRaycast(Vector3 startPos)
    {
        if (IsPointerOverUI())
        {
            return null;
        }

        Ray ray = Managers.Camera.ScreenPointToRay(startPos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, Constants.Layers.Hexagons))
        {
            return hit.collider.transform?.parent?.GetComponent<HexagonMono>();
        }

        return null;
    }

    public static bool IsPointerOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    public static void SetMaterialsRecursively(this GameObject gameobject, Material material)
    {
        foreach (MeshRenderer renderer in gameobject.GetComponentsInChildren<MeshRenderer>())
        {
            renderer.material = material;
        }
    }

    public static float PerlinNoise(float x, float y, float scale, int seed)
    {
        return Mathf.PerlinNoise(x / scale + seed, y / scale + seed);
    }

    public static bool IsInBounds(Vector2Int position, RectInt dimensions)
    {
        return IsInBounds(position, dimensions.max);
    }

    public static bool IsInBounds(Vector2Int position, Vector2Int dimensions)
    {
        if (position.x < 0 || position.x >= dimensions.x)
        {
            return false;
        }

        if (position.y < 0 || position.y >= dimensions.y)
        {
            return false;
        }

        return true;
    }

    public static void TriggerAllParticleSystems(Transform transform, bool start)
    {
        if (transform == null)
        {
            return;
        }

        transform.gameObject.TryGetComponent<ParticleSystem>(out ParticleSystem parentPS);
        parentPS?.Stop();
        foreach (ParticleSystem ps in transform.GetComponentsInChildren<ParticleSystem>())
        {
            if (start)
            {
                ps.Play();
            }
            else
            {
                ps.Stop();
            }
        }
    }

    private static readonly List<Vector2Int> oddNeighborPattern = new List<Vector2Int>()
    {
        new Vector2Int(-1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(1, 0),
        new Vector2Int(1, 1),
        new Vector2Int(0, 1),
        new Vector2Int(-1, 1)
    };

    private static readonly List<Vector2Int> evenNeighborPattern = new List<Vector2Int>()
    {
        new Vector2Int(-1,-1),
        new Vector2Int(0, -1),
        new Vector2Int(1, -1),
        new Vector2Int(1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(-1, 0)
    };

    public static Vector2Int GetNeighborPosition(int x, int y, HexSide direction)
    {
        return GetNeighborPosition(new Vector2Int(x, y), direction);
    }

    public static Vector2Int GetNeighborPosition(Vector2Int pos, HexSide direction)
    {
        Vector2Int position;

        if (pos.x % 2 == 0)
        {
            position = pos + evenNeighborPattern[(int)direction];
        }
        else
        {
            position = pos + oddNeighborPattern[(int)direction];
        }

        return position;
    }

    public static Transform RecursiveFindChild(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
            {
                return child;
            }
            else
            {
                Transform found = RecursiveFindChild(child, childName);
                if (found != null)
                {
                    return found;
                }
            }
        }

        return null;
    }

    public static Vector3 ToWorldPosition(int x, int y)
    {
        float xF = x * Constants.HorizontalDistanceBetweenHexagons;
        float zF = y * Constants.VerticalDistanceBetweenHexagons + (x % 2 == 1 ? Constants.HEXAGON_r : 0);
        float yF = 0;
        if (Helpers.GetFromChunk(Managers.Board.LoadedChunks, x, y, out Hexagon hex))
        {
            zF = hex.Height;
        }
        return new Vector3(xF, yF, zF);
    }

    public static Vector3 ToWorldPosition(Vector2Int position)
    {
        return ToWorldPosition(position.x, position.y);
    }

    public static Vector2Int ToGridPosition(Vector3 pos)
    {
        int x = Mathf.RoundToInt(pos.x / Constants.HorizontalDistanceBetweenHexagons);
        int y = Mathf.RoundToInt((pos.z - (x % 2 == 1 ? Constants.HEXAGON_r : 0)) / Constants.VerticalDistanceBetweenHexagons);
        return new Vector2Int(x, y);
    }

    public static Vector3 ToOverworldPosition(Vector2Int position)
    {
        return ToOverworldPosition(position.x, position.y);
    }

    public static Vector3 ToOverworldPosition(int x, int y)
    {
        Vector3 pos = new Vector3(
            x * Constants.OverworldHorizontalDistanceBetweenHexagons,
            0,
            y * Constants.OverworldVerticalDistanceBetweenHexagons);
        if (x % 2 == 1)
        {
            pos.z += Constants.OverworldHorizontalDistanceBetweenHexagons / 2;
        }

        return pos;
    }

    public static int RoundTo25(int value)
    {
        return (value / 25 + (value % 25 > 12 ? 1 : 0)) * 25;
    }

    public static float AngleDir(Vector3 fwd, Vector3 targetDir)
    {
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, Vector3.up);

        if (dir > 0.0)
        {
            return 1.0f;
        }
        else if (dir < 0.0)
        {
            return -1.0f;
        }
        else
        {
            return 0.0f;
        }
    }

    public static float AngleXZ(Vector3 forward, Vector3 targetDirection)
    {
        forward.y = 0;
        targetDirection.y = 0;
        return Vector3.Angle(targetDirection, forward);
    }

    public static void GetHexNeighbors(Vector2Int pos, int mapDimensions, Func<int, int, bool> forNeighbor)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>(8);
        for (int x = pos.x - 1; x <= pos.x + 1; x++)
        {
            for (int y = pos.y - 1; y <= pos.y + 1; y++)
            {
                if (x < 0 || x >= mapDimensions || y < 0 || y >= mapDimensions)
                {
                    continue;
                }

                if (x == pos.x && y == pos.y)
                {
                    continue;
                }

                forNeighbor(x, y);
            }
        }
    }

    public static List<Vector2Int> GetHexesInRange(Vector2Int startPos, int range)
    {
        if (range > 1)
        {
            throw new NotImplementedException("Sorry I'm lazy");
        }

        if (range == 0)
        {
            return new List<Vector2Int> { startPos };
        }
        else
        {
            List<Vector2Int> hexes = new List<Vector2Int>();
            for (int i = 0; i < 6; i++)
            {
                hexes.Add(GetNeighborPosition(startPos, (HexSide)i));
            }
            return hexes;
        }
    }

    public static bool GetFromChunk<T>(Dictionary<Vector2Int, T[,]> chunks, int x, int y, out T hex)
    {
        Vector2Int chunkIndex = new Vector2Int(x / Constants.CHUNK_SIZE, y / Constants.CHUNK_SIZE);
        if (!chunks.ContainsKey(chunkIndex))
        {
            hex = default(T);
            return false;
        }
        else
        {
            hex = chunks[chunkIndex][x - chunkIndex.x * Constants.CHUNK_SIZE, y - chunkIndex.y * Constants.CHUNK_SIZE];
            return true;
        }
    }

    public static void SetInChunk<T>(Dictionary<Vector2Int, T[,]> chunks, int x, int y, T val)
    {
        Vector2Int chunkIndex = new Vector2Int(x / Constants.CHUNK_SIZE, y / Constants.CHUNK_SIZE);
        if (!chunks.ContainsKey(chunkIndex))
        {
            return;
        }
        else
        {
            chunks[chunkIndex][x - chunkIndex.x * Constants.CHUNK_SIZE, y - chunkIndex.y * Constants.CHUNK_SIZE] = val;
        }
    }

}