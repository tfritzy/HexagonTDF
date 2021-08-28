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
        if (IsPointerOverUIObject())
        {
            // UI was clicked, and we don't want to go through it.
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

    private static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public static void SetMaterialsRecursively(this GameObject gameobject, Material material)
    {
        foreach (MeshRenderer renderer in gameobject.GetComponentsInChildren<MeshRenderer>())
        {
            renderer.material = material;
        }
    }

    public static List<Vector2Int> FindPathByWalking(Vector2Int sourcePos, Vector2Int endPos, HexagonMono[,] hexes)
    {
        return FindPath(hexes, sourcePos, endPos, (Vector2Int sPos, Vector2Int ePos) => IsPosWalkable(sPos, ePos, hexes));
    }

    public static List<Vector2Int> FindPath(HexagonMono[,] map, Vector2Int sourcePos, Vector2Int endPos, Func<Vector2Int, Vector2Int, bool> shouldInclude)
    {
        return FindPath(map, sourcePos, new HashSet<Vector2Int>() { endPos }, shouldInclude);
    }

    public static List<Vector2Int> FindPath(
        HexagonMono[,] map,
        Vector2Int sourcePos,
        HashSet<Vector2Int> endPos,
        Func<Vector2Int, Vector2Int, bool> shouldInclude)
    {
        Queue<PredGridPoint> q = new Queue<PredGridPoint>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        PredGridPoint[,] predecessorGrid = BuildPredecessorGrid(map.GetLength(0), map.GetLength(1));
        q.Enqueue(new PredGridPoint(sourcePos, 0));

        while (q.Count > 0)
        {
            PredGridPoint current = q.Dequeue();
            if (visited.Contains(current.Position))
            {
                continue;
            }

            visited.Add(current.Position);
            for (int i = 0; i < 6; i++)
            {
                Vector2Int testPosition = GetNeighborPosition(map, current.Position, i);
                if (testPosition == Constants.MinVector2Int)
                {
                    continue;
                }

                if (predecessorGrid[testPosition.x, testPosition.y].Position == Constants.MaxVector2Int)
                {
                    predecessorGrid[testPosition.x, testPosition.y] = current;
                }

                if (visited.Contains(testPosition) || shouldInclude(current.Position, testPosition) == false)
                {
                    continue;
                }

                if (endPos.Contains(testPosition))
                {
                    return GetPathFromPredecessorGrid(predecessorGrid, sourcePos, testPosition);
                }

                q.Enqueue(new PredGridPoint(testPosition, current.Distance + 1));
            }
        }

        return null;
    }

    public static PredGridPoint[,] GetPredicessorGridWalking(
        HexagonMono[,] map,
        Vector2Int sourcePos)
    {
        return GetPredecessorGrid(
            map, sourcePos, (Vector2Int sPos, Vector2Int ePos) =>
                IsPosWalkable(sPos, ePos, map) || sPos == sourcePos || ePos == sourcePos
        );
    }

    public static PredGridPoint[,] GetPredecessorGrid(
        HexagonMono[,] map,
        Vector2Int sourcePos,
        Func<Vector2Int, Vector2Int, bool> shouldInclude)
    {
        Queue<PredGridPoint> q = new Queue<PredGridPoint>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        PredGridPoint[,] predecessorGrid = BuildPredecessorGrid(map.GetLength(0), map.GetLength(1));

        if (shouldInclude(Constants.MaxVector2Int, sourcePos) == false)
        {
            return predecessorGrid;
        }

        q.Enqueue(new PredGridPoint(sourcePos, 0));

        while (q.Count > 0)
        {
            PredGridPoint current = q.Dequeue();
            if (visited.Contains(current.Position))
            {
                continue;
            }

            visited.Add(current.Position);
            for (int i = 0; i < 6; i++)
            {
                Vector2Int testPosition = GetNeighborPosition(map, current.Position, i);
                if (testPosition == Constants.MinVector2Int)
                {
                    continue;
                }

                if (visited.Contains(testPosition) || shouldInclude(current.Position, testPosition) == false)
                {
                    continue;
                }

                if (predecessorGrid[testPosition.x, testPosition.y].Position == Constants.MaxVector2Int)
                {
                    predecessorGrid[testPosition.x, testPosition.y] = current;
                }

                q.Enqueue(new PredGridPoint(testPosition, current.Distance + 1));
            }
        }

        return predecessorGrid;
    }

    public static List<HashSet<Vector2Int>> FindCongruentGroups(HexagonMono[,] map, HashSet<Vector2Int> allPositions, Func<Vector2Int, bool> shouldInclude)
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

    public static HashSet<Vector2Int> GetCongruentHexes(HexagonMono[,] map, Vector2Int startingPos, Func<Vector2Int, bool> shouldInclude)
    {
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        DFS(map, startingPos, visited, 0, int.MaxValue, shouldInclude);
        return visited;
    }

    private static void DFS(HexagonMono[,] map, Vector2Int position, HashSet<Vector2Int> visited, int currentHops, int maxHops, Func<Vector2Int, bool> shouldInclude)
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

    private static PredGridPoint[,] BuildPredecessorGrid(int sizeX, int sizeY)
    {
        PredGridPoint[,] predecessorGrid = new PredGridPoint[sizeX, sizeY];
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                predecessorGrid[x, y] = new PredGridPoint();
            }
        }

        return predecessorGrid;
    }

    private static List<Vector2Int> GetPathFromPredecessorGrid(PredGridPoint[,] grid, Vector2Int startPosition, Vector2Int endPosition)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int current = endPosition;
        while (current != Constants.MaxVector2Int)
        {
            path.Add(current);
            current = grid[current.x, current.y].Position;

            if (current == startPosition)
            {
                path.Add(current);
                break;
            }
        }

        path.Reverse();
        return path;
    }

    public static float PerlinNoise(float x, float y, float scale, int seed)
    {
        return Mathf.PerlinNoise(x / scale + seed, y / scale + seed);
    }

    public static bool IsInBounds(HexagonMono[,] map, Vector2Int position)
    {
        if (position.x < 0 || position.x >= map.GetLength(0))
        {
            return false;
        }

        if (position.y < 0 || position.y >= map.GetLength(1))
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

    public static Vector2Int GetNeighborPosition(HexagonMono[,] map, Vector2Int pos, int index)
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

    public static bool IsWithinRange(Vector2Int sourcePos, Vector2Int targetPos, int range)
    {
        return Managers.Board.GetFlightDistanceToTarget(targetPos, sourcePos) <= range;
    }

    public static Vector3 ToWorldPosition(int x, int y)
    {
        float xF = x * Constants.HorizontalDistanceBetweenHexagons;
        float zF = y * Constants.VerticalDistanceBetweenHexagons + (x % 2 == 1 ? Constants.HEXAGON_r : 0);
        return new Vector3(xF, 0f, zF);
    }

    public static Vector3 ToWorldPosition(Vector2Int position)
    {
        return ToWorldPosition(position.x, position.y);
    }

    public static bool IsPosWalkable(Vector2Int startPos, Vector2Int endPos, HexagonMono[,] hexes)
    {
        return IsInBounds(hexes, startPos) && IsInBounds(hexes, endPos)
            && hexes[startPos.x, startPos.y].transform.position.y == hexes[endPos.x, endPos.y].transform.position.y
            && hexes[endPos.x, endPos.y].IsWalkable
            && (Managers.Board.Buildings.ContainsKey(endPos) == false || Managers.Board.Buildings[endPos].IsWalkable);
    }

    public static int RoundTo25(int value)
    {
        return (value / 25 + (value % 25 > 12 ? 1 : 0)) * 25;
    }
}