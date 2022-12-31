using System;
using System.Collections.Generic;
using UnityEngine;

public class Navigation
{
    public List<Vector2Int> Terminations { get; private set; }
    private NavMapPoint[,] NavMap;
    private NavMapPoint[,] IdealNavMap;
    private TownHall townHall;
    private Vector2Int dimensions;

    struct NavMapPoint
    {
        public int Distance;
        public Vector2Int Next;
        public bool IsTermination;
    }

    public Navigation(Vector2Int boardSize, TownHall townHall)
    {
        this.NavMap = new NavMapPoint[boardSize.x, boardSize.y];
        this.IdealNavMap = new NavMapPoint[boardSize.x, boardSize.y];
        this.townHall = townHall;
        this.dimensions = boardSize;
    }

    public int GetDistanceToTownHall(Vector2Int pos)
    {
        return NavMap[pos.x, pos.y].Distance;
    }

    public int GetIdealDistanceToTownHall(Vector2Int pos)
    {
        return IdealNavMap[pos.x, pos.y].Distance;
    }

    public Vector2Int GetNextPos(Vector2Int currentPos)
    {
        return NavMap[currentPos.x, currentPos.y].Next;
    }
    public Vector2Int GetIdealNextPos(Vector2Int currentPos)
    {
        return IdealNavMap[currentPos.x, currentPos.y].Next;
    }


    public void ReacalculatePath(Hexagon[,] segment, Building[,] bulidings)
    {
        Dijkstra(this.NavMap, segment, bulidings);
    }

    public void ReacalculateIdealPath(Hexagon[,] segment, Building[,] bulidings)
    {
        Dijkstra(this.IdealNavMap, segment, bulidings, ignorePlayerObstacles: true);
    }

    private void Dijkstra(NavMapPoint[,] navMap, Hexagon[,] segment, Building[,] bulidings, bool ignorePlayerObstacles = false)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        InitNavMap(navMap, segment);
        navMap[townHall.GridPosition.x, townHall.GridPosition.y].Distance = 0;
        queue.Enqueue(this.townHall.GridPosition);

        foreach (HexSide side in townHall.ExtraSize)
        {
            Vector2Int point = Helpers.GetNeighborPosition(townHall.GridPosition, side);
            navMap[point.x, point.y].Distance = 0;
            queue.Enqueue(point);
        }

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            for (int i = 0; i < 6; i++)
            {
                Vector2Int neighbor = Helpers.GetNeighborPosition(current, (HexSide)i);
                if (!Helpers.IsInBounds(neighbor, this.dimensions) ||
                    !segment[neighbor.x, neighbor.y].IsWalkable)
                {
                    continue;
                }

                if (!ignorePlayerObstacles && bulidings[neighbor.x, neighbor.y] != null)
                {
                    continue;
                }

                if (navMap[neighbor.x, neighbor.y].Distance > navMap[current.x, current.y].Distance + 1)
                {
                    queue.Enqueue(neighbor);
                    navMap[neighbor.x, neighbor.y].Distance = navMap[current.x, current.y].Distance + 1;
                    navMap[neighbor.x, neighbor.y].Next = current;
                    navMap[current.x, current.y].IsTermination = false;
                }
            }
        }

        Terminations = new List<Vector2Int>();
        for (int x = 0; x < dimensions.x; x++)
        {
            for (int y = 0; y < dimensions.y; y++)
            {
                if (navMap[x, y].Distance == Constants.MAX_ISH)
                {
                    navMap[x, y].IsTermination = false;
                }

                if (navMap[x, y].IsTermination)
                {
                    Terminations.Add(new Vector2Int(x, y));
                }
            }
        }
    }

    struct BFSPoint
    {
        public int Distance;
        public Vector2Int Next;
        public bool IsTermination;
    }

    public LinkedList<Vector2Int> BFS(Vector2Int startPos, Vector2Int targetPos, Hexagon[,] segment, Building[,] bulidings)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        bool[,] visited = new bool[segment.GetLength(0), segment.GetLength(1)];
        Dictionary<Vector2Int, Vector2Int> nextMap = new Dictionary<Vector2Int, Vector2Int>();
        queue.Enqueue(startPos);

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            for (int i = 0; i < 6; i++)
            {
                Vector2Int neighbor = Helpers.GetNeighborPosition(current, (HexSide)i);

                if (!Helpers.IsInBounds(neighbor, this.dimensions) ||
                    !segment[neighbor.x, neighbor.y].IsWalkable)
                {
                    continue;
                }

                if (neighbor == targetPos)
                {
                    nextMap[neighbor] = current;
                    return GetPathFromMap(nextMap, targetPos, startPos);
                }

                if (!visited[neighbor.x, neighbor.y])
                {
                    queue.Enqueue(neighbor);
                    visited[neighbor.x, neighbor.y] = true;
                    nextMap[neighbor] = current;
                }
            }
        }

        return new LinkedList<Vector2Int>();
    }

    private LinkedList<Vector2Int> GetPathFromMap(Dictionary<Vector2Int, Vector2Int> map, Vector2Int endPos, Vector2Int startPos)
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