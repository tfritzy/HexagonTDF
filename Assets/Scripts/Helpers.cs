using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Helpers
{
    public static Hexagon FindHexByRaycast(Vector3 startPos)
    {
        Ray ray = Managers.Camera.ScreenPointToRay(startPos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, Constants.Layers.Hexagons))
        {
            return hit.collider.transform?.parent?.GetComponent<Hexagon>();
        }

        return null;
    }

    public static void SetMaterialsRecursively(this GameObject gameobject, Material material)
    {
        foreach (MeshRenderer renderer in gameobject.GetComponentsInChildren<MeshRenderer>())
        {
            renderer.material = material;
        }
    }

    public static List<Vector2Int> FindPath(Hexagon[,] grid, Dictionary<Vector2Int, BuildingType> buildings, Vector2Int sourcePos, Vector2Int endPos)
    {
        return FindPath(grid, buildings, sourcePos, new HashSet<Vector2Int>() { endPos });
    }

    public static List<Vector2Int> FindPath(Hexagon[,] grid, Dictionary<Vector2Int, BuildingType> buildings, Vector2Int sourcePos, HashSet<Vector2Int> endPos)
    {
        Queue<Vector2Int> q = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Vector2Int[,] predecessorGrid = BuildPredecessorGrid(grid.GetLength(0), grid.GetLength(1));
        q.Enqueue(sourcePos);

        while (q.Count > 0)
        {
            Vector2Int current = q.Dequeue();
            if (visited.Contains(current))
            {
                continue;
            }

            visited.Add(current);
            for (int i = 0; i < 6; i++)
            {
                Vector2Int testPosition = grid[current.x, current.y].GetNeighborPosition(i);
                if (testPosition == Constants.MinVector2Int)
                {
                    continue;
                }

                if (predecessorGrid[testPosition.x, testPosition.y] == Constants.MaxVector2Int)
                {
                    predecessorGrid[testPosition.x, testPosition.y] = current;
                }

                if (endPos.Contains(testPosition))
                {
                    return GetPathFromPredecessorGrid(predecessorGrid, sourcePos, testPosition);
                }

                if (visited.Contains(testPosition) || IsTraversable(testPosition, grid, buildings) == false)
                {
                    continue;
                }

                q.Enqueue(testPosition);
            }
        }

        return null;
    }

    private static bool IsTraversable(Vector2Int position, Hexagon[,] grid, Dictionary<Vector2Int, BuildingType> buildings)
    {
        return Managers.Map.IsBuildable(position);
    }

    public static List<Vector2Int> GetAllHexInRange(Vector2Int position, int range)
    {
        Dictionary<Vector2Int, int> visited = new Dictionary<Vector2Int, int>();
        Queue<Vector3Int> q = new Queue<Vector3Int>();
        q.Enqueue(new Vector3Int(position.x, position.y, 0));

        while (q.Count > 0)
        {
            Vector3Int current = q.Dequeue();
            if (visited.ContainsKey((Vector2Int)current))
            {
                continue;
            }

            if (current.z > range)
            {
                continue;
            }

            visited[(Vector2Int)current] = current.z;

            for (int i = 0; i < 6; i++)
            {
                Vector2Int neighbor = Managers.Map.Hexagons[current.x, current.y].GetNeighbor(i).GridPosition;
                q.Enqueue(new Vector3Int(neighbor.x, neighbor.y, current.z + 1));
            }
        }

        return visited.Keys.ToList();
    }

    private static void DFS(Vector2Int position, HashSet<Vector2Int> visited, int currentHops, int maxHops)
    {
        if (currentHops > maxHops)
        {
            return;
        }

        if (visited.Contains(position))
        {
            return;
        }

        visited.Add(position);
        for (int i = 0; i < 6; i++)
        {
            Vector2Int neighborPos = Managers.Map.Hexagons[position.x, position.y].GetNeighborPosition(i);
            if (neighborPos == Constants.MinVector2Int)
            {
                continue;
            }

            DFS(neighborPos, visited, currentHops + 1, maxHops);
        }
    }

    private static Vector2Int[,] BuildPredecessorGrid(int sizeX, int sizeY)
    {
        Vector2Int[,] predecessorGrid = new Vector2Int[sizeX, sizeY];
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                predecessorGrid[x, y] = Constants.MaxVector2Int;
            }
        }

        return predecessorGrid;
    }

    private static List<Vector2Int> GetPathFromPredecessorGrid(Vector2Int[,] grid, Vector2Int startPosition, Vector2Int endPosition)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int current = grid[endPosition.x, endPosition.y];
        while (current != Constants.MaxVector2Int)
        {
            path.Add(current);
            current = grid[current.x, current.y];

            if (current == startPosition)
            {
                break;
            }
        }

        path.Reverse();
        path.Add(endPosition);

        return path;
    }

    public static bool IsInBounds(Vector2Int position)
    {
        if (position.x < 0 || position.x >= Managers.Map.Hexagons.GetLength(0))
        {
            return false;
        }

        if (position.y < 0 || position.y >= Managers.Map.Hexagons.GetLength(1))
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
}