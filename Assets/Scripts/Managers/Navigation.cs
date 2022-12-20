using System;
using System.Collections.Generic;
using UnityEngine;

public class Navigation
{
    private NavMapPoint[,] NavMap;
    private Vector2Int townHallPos;
    private Vector2Int dimensions;

    struct NavMapPoint
    {
        public int Distance;
        public Vector2Int Next;
    }

    public Navigation(Vector2Int boardSize, Vector2Int townHallPos)
    {
        NavMap = new NavMapPoint[boardSize.x, boardSize.y];
        this.townHallPos = townHallPos;
        this.dimensions = boardSize;
    }

    public Vector2Int GetNextPos(Vector2Int currentPos)
    {
        return NavMap[currentPos.x, currentPos.y].Next;
    }

    public void ReacalculatePath(Hexagon[,] segment)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        NavMapPoint[,] navMap = InitNavMap(segment);
        navMap[townHallPos.x, townHallPos.y].Distance = 0;
        queue.Enqueue(this.townHallPos);

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

                if (navMap[neighbor.x, neighbor.y].Distance > navMap[current.x, current.y].Distance + 1)
                {
                    queue.Enqueue(neighbor);
                    navMap[neighbor.x, neighbor.y].Distance = navMap[current.x, current.y].Distance + 1;
                    navMap[neighbor.x, neighbor.y].Next = current;
                }
            }
        }

        this.NavMap = navMap;
    }

    private NavMapPoint[,] InitNavMap(Hexagon[,] points)
    {
        NavMapPoint[,] distance = new NavMapPoint[points.GetLength(0), points.GetLength(1)];

        for (int x = 0; x < distance.GetLength(0); x++)
        {
            for (int y = 0; y < distance.GetLength(1); y++)
            {
                distance[x, y] = new NavMapPoint
                {
                    Distance = int.MaxValue / 2,
                    Next = Constants.MaxVector2Int,
                };
            }
        }

        return distance;
    }
}