using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Helpers
{
    public static HexagonMono FindHexByRaycast(Vector3 startPos)
    {
        Ray ray = Managers.Camera.ScreenPointToRay(startPos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, Constants.Layers.Hexagons))
        {
            return hit.collider.transform?.parent?.GetComponent<HexagonMono>();
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

    public static List<Vector2Int> FindPath(Map map, Vector2Int sourcePos, Vector2Int endPos, Func<Vector2Int, bool> shouldInclude)
    {
        return FindPath(map, sourcePos, new HashSet<Vector2Int>() { endPos }, shouldInclude, (Vector2Int pos) => { return true; });
    }

    public static List<Vector2Int> FindPath(Vector2Int[,] predecessorGrid, Vector2Int startPos, Vector2Int endPos)
    {
        return GetPathFromPredecessorGrid(predecessorGrid, startPos, endPos);
    }

    public static List<Vector2Int> FindPath(Map map, Vector2Int sourcePos, HashSet<Vector2Int> endPos, Func<Vector2Int, bool> shouldInclude)
    {
        return FindPath(map, sourcePos, endPos, shouldInclude, (Vector2Int pos) => { return true; });
    }

    public static List<Vector2Int> FindPath(Map map, Vector2Int sourcePos, HashSet<Vector2Int> endPos, Func<Vector2Int, bool> shouldInclude, Func<Vector2Int, bool> isValidEnd)
    {
        Queue<Vector2Int> q = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Vector2Int[,] predecessorGrid = BuildPredecessorGrid(map.Width, map.Height);
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
                Vector2Int testPosition = GetNeighborPosition(map, current, i);
                if (testPosition == Constants.MinVector2Int)
                {
                    continue;
                }

                if (predecessorGrid[testPosition.x, testPosition.y] == Constants.MaxVector2Int)
                {
                    predecessorGrid[testPosition.x, testPosition.y] = current;
                }

                if (endPos.Contains(testPosition) && isValidEnd(testPosition))
                {
                    return GetPathFromPredecessorGrid(predecessorGrid, sourcePos, testPosition);
                }

                if (visited.Contains(testPosition) || shouldInclude(testPosition) == false)
                {
                    continue;
                }

                q.Enqueue(testPosition);
            }
        }

        return null;
    }


    public static Vector2Int[,] GetPredecessorGrid(Map map, Vector2Int sourcePos, Func<Vector2Int, bool> shouldInclude)
    {
        Queue<Vector2Int> q = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Vector2Int[,] predecessorGrid = BuildPredecessorGrid(map.Width, map.Height);
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
                Vector2Int testPosition = GetNeighborPosition(map, current, i);
                if (testPosition == Constants.MinVector2Int)
                {
                    continue;
                }

                if (visited.Contains(testPosition) || shouldInclude(testPosition) == false)
                {
                    continue;
                }

                if (predecessorGrid[testPosition.x, testPosition.y] == Constants.MaxVector2Int)
                {
                    predecessorGrid[testPosition.x, testPosition.y] = current;
                }

                q.Enqueue(testPosition);
            }
        }

        return predecessorGrid;
    }

    private static bool IsTraversable(Vector2Int position, HexagonMono[,] grid, Dictionary<Vector2Int, Building> buildings)
    {
        return (buildings.ContainsKey(position) == false || buildings[position].IsWalkable) && Managers.Board.Hexagons[position.x, position.y].IsWalkable;
    }

    public static bool IsTraversable(Vector2Int position)
    {
        return IsTraversable(position, Managers.Board.Hexagons, Managers.Board.Buildings);
    }

    public static List<Vector2Int> GetAllHexInRange(Map map, Vector2Int position, int range)
    {
        Dictionary<Vector2Int, int> visited = new Dictionary<Vector2Int, int>();
        Queue<Vector3Int> q = new Queue<Vector3Int>();
        q.Enqueue(new Vector3Int(position.x, position.y, 0)); // z == number of hops.

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
                Vector2Int neighbor = GetNeighborPosition(map, (Vector2Int)current, i);
                q.Enqueue(new Vector3Int(neighbor.x, neighbor.y, current.z + 1));
            }
        }

        return visited.Keys.ToList();
    }

    public static List<HashSet<Vector2Int>> FindCongruentGroups(Map map, HashSet<Vector2Int> allPositions, Func<Vector2Int, bool> shouldInclude)
    {
        List<HashSet<Vector2Int>> groups = new List<HashSet<Vector2Int>>();
        Queue<Vector2Int> toTraverse = new Queue<Vector2Int>(allPositions);

        while (toTraverse.Count > 0)
        {
            HashSet<Vector2Int> group = new HashSet<Vector2Int>();
            Vector2Int position = toTraverse.Dequeue();
            if (allPositions.Contains(position) == false)
            {
                continue;
            }

            DFS(map, position, group, 0, int.MaxValue, shouldInclude);
            foreach (Vector2Int pos in group)
            {
                allPositions.Remove(pos);
            }
            groups.Add(group);
        }

        groups.Sort((HashSet<Vector2Int> s1, HashSet<Vector2Int> s2) => { return s2.Count - s1.Count; });

        return groups;
    }

    public static HashSet<Vector2Int> GetCongruentHexes(Map map, Vector2Int startingPos, Func<Vector2Int, bool> shouldInclude)
    {
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        DFS(map, startingPos, visited, 0, int.MaxValue, shouldInclude);
        return visited;
    }

    private static void DFS(Map map, Vector2Int position, HashSet<Vector2Int> visited, int currentHops, int maxHops, Func<Vector2Int, bool> shouldInclude)
    {
        if (currentHops > maxHops)
        {
            return;
        }

        if (visited.Contains(position))
        {
            return;
        }

        if (shouldInclude != null && shouldInclude(position) == false)
        {
            return;
        }

        visited.Add(position);
        for (int i = 0; i < 6; i++)
        {
            Vector2Int neighborPos = GetNeighborPosition(map, position, i);
            if (neighborPos == Constants.MinVector2Int)
            {
                continue;
            }

            DFS(map, neighborPos, visited, currentHops + 1, maxHops, shouldInclude);
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

    public static bool IsInBounds(Map map, Vector2Int position)
    {
        if (position.x < 0 || position.x >= map.Width)
        {
            return false;
        }

        if (position.y < 0 || position.y >= map.Height)
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

    public static Vector2Int GetNeighborPosition(Map map, Vector2Int pos, int index)
    {
        Vector2Int position;

        if (pos.x % 2 == 0)
        {
            position = pos + evenNeighborPattern[index];
        }
        else
        {
            position = pos + oddNeighborPattern[index];
        }

        if (Helpers.IsInBounds(map, position))
        {
            return position;
        }
        else
        {
            return Constants.MinVector2Int;
        }
    }
}