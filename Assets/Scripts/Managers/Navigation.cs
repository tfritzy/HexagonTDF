using System;
using System.Collections.Generic;
using UnityEngine;

public class Navigation
{
    public List<Vector2Int> Terminations { get; private set; }
    private TownHall townHall;

    struct NavMapPoint
    {
        public int Distance;
        public Vector2Int Next;
        public bool IsTermination;
    }

    public Navigation(TownHall townHall)
    {
        this.townHall = townHall;
    }

    // private void Dijkstra(NavMapPoint[,] navMap, Hexagon[,] segment, Building[,] bulidings, bool ignorePlayerObstacles = false)
    // {
    //     Queue<Vector2Int> queue = new Queue<Vector2Int>();
    //     InitNavMap(navMap, segment);
    //     navMap[townHall.GridPosition.x, townHall.GridPosition.y].Distance = 0;
    //     queue.Enqueue(this.townHall.GridPosition);

    //     foreach (HexSide side in townHall.ExtraSize)
    //     {
    //         Vector2Int point = Helpers.GetNeighborPosition(townHall.GridPosition, side);
    //         navMap[point.x, point.y].Distance = 0;
    //         queue.Enqueue(point);
    //     }

    //     while (queue.Count > 0)
    //     {
    //         Vector2Int current = queue.Dequeue();

    //         for (int i = 0; i < 6; i++)
    //         {
    //             Vector2Int neighbor = Helpers.GetNeighborPosition(current, (HexSide)i);
    //             if (!Helpers.IsInBounds(neighbor, this.dimensions) ||
    //                 !segment[neighbor.x, neighbor.y].IsWalkable)
    //             {
    //                 continue;
    //             }

    //             if (!ignorePlayerObstacles && bulidings[neighbor.x, neighbor.y] != null)
    //             {
    //                 continue;
    //             }

    //             if (navMap[neighbor.x, neighbor.y].Distance > navMap[current.x, current.y].Distance + 1)
    //             {
    //                 queue.Enqueue(neighbor);
    //                 navMap[neighbor.x, neighbor.y].Distance = navMap[current.x, current.y].Distance + 1;
    //                 navMap[neighbor.x, neighbor.y].Next = current;
    //                 navMap[current.x, current.y].IsTermination = false;
    //             }
    //         }
    //     }

    //     Terminations = new List<Vector2Int>();
    //     for (int x = 0; x < dimensions.x; x++)
    //     {
    //         for (int y = 0; y < dimensions.y; y++)
    //         {
    //             if (navMap[x, y].Distance == Constants.MAX_ISH)
    //             {
    //                 navMap[x, y].IsTermination = false;
    //             }

    //             if (navMap[x, y].IsTermination)
    //             {
    //                 Terminations.Add(new Vector2Int(x, y));
    //             }
    //         }
    //     }
    // }

    struct BFSPoint
    {
        public int Distance;
        public Vector2Int Next;
        public bool IsTermination;
    }

    public static LinkedList<Vector2Int> BFS(Vector2Int startPos, Vector2Int targetPos, World world)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        // visited[startPos.x, startPos.y] = true;
        Dictionary<Vector2Int, Vector2Int> nextMap = new Dictionary<Vector2Int, Vector2Int>();
        queue.Enqueue(startPos);

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            for (int i = 0; i < 6; i++)
            {
                Vector2Int neighbor = Helpers.GetNeighborPosition(current, (HexSide)i);

                if (neighbor == targetPos)
                {
                    nextMap[neighbor] = current;
                    return GetPathFromMap(nextMap, targetPos, startPos);
                }

                Hexagon topHex = world.GetTopHex(neighbor.x, neighbor.y);
                if (topHex == null || !topHex.IsWalkable)
                {
                    continue;
                }

                if (!visited.Contains(neighbor))
                {
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                    nextMap[neighbor] = current;
                }
            }
        }

        return new LinkedList<Vector2Int>();
    }

    private static LinkedList<Vector2Int> GetPathFromMap(Dictionary<Vector2Int, Vector2Int> map, Vector2Int endPos, Vector2Int startPos)
    {
        LinkedList<Vector2Int> path = new LinkedList<Vector2Int>();
        Vector2Int iterPos = endPos;
        while (iterPos != startPos)
        {
            path.AddFirst(iterPos);

            if (!map.ContainsKey(iterPos))
            {
                break;
            }

            iterPos = map[iterPos];
        }

        return path;
    }

    private void InitNavMap(NavMapPoint[,] navMap, Hexagon[,] points)
    {
        for (int x = 0; x < navMap.GetLength(0); x++)
        {
            for (int y = 0; y < navMap.GetLength(1); y++)
            {
                navMap[x, y].Distance = Constants.MAX_ISH;
                navMap[x, y].Next = Constants.MaxVector2Int;
                navMap[x, y].IsTermination = true;
            }
        }
    }
}