using System;
using System.Collections.Generic;
using UnityEngine;

public class Navigation
{
    private HexSide[,] NextMap;
    private Vector2Int townHallPos;

    public Navigation(Vector2Int boardSize, Vector2Int townHallPos)
    {
        NextMap = new HexSide[boardSize.x, boardSize.y];
        this.townHallPos = townHallPos;
    }

    private void ReacalculatePath(Hexagon[,] segment)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        int[,] distance = InitDistArray(segment);
        distance[townHallPos.x, townHallPos.y] = 0;
        queue.Enqueue(this.townHallPos);

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            for (int i = 0; i < 6; i++)
            {
                Vector2Int neighbor = Helpers.GetNeighborPosition(current, (HexSide)i);
                if (distance[neighbor.x, neighbor.y] > distance[current.x, current.y] + 1)
                {
                    queue.Enqueue(neighbor);
                    distance[neighbor.x, neighbor.y] = distance[current.x, current.y] + 1;
                }
            }
        }
    }

    private int[,] InitDistArray(Hexagon[,] points)
    {
        int[,] distance = new int[points.GetLength(0), points.GetLength(1)];

        for (int x = 0; x < distance.GetLength(0); x++)
        {
            for (int y = 0; y < distance.GetLength(1); y++)
            {
                distance[x, y] = int.MaxValue;
            }
        }

        return distance;
    }
}