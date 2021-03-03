using System.Collections.Generic;
using UnityEngine;

public static class Helpers
{
    public static Hexagon FindHexByRaycast()
    {
        Ray ray = Managers.Camera.ScreenPointToRay(Input.mousePosition);
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

    public static List<Vector2Int> FindPath(Hexagon[,] grid, Vector2Int sourcePos, Vector2Int endPos)
    {
        Queue<Vector2Int> q = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Vector2Int[,] predecessorGrid = BuildPredecessorGrid(grid.GetLength(0), grid.GetLength(1));
        q.Enqueue(sourcePos);

        while (q.Count > 0)
        {
            Vector2Int current = q.Dequeue();
            visited.Add(current);
            for (int i = 0; i < 6; i++)
            {
                Vector2Int testPosition = grid[current.x, current.y].GetNeighborPosition(i);
                if (visited.Contains(testPosition) || grid[testPosition.x, testPosition.y].IsTraversable == false)
                {
                    continue;
                }

                predecessorGrid[testPosition.x, testPosition.y] = current;
                if (testPosition == endPos)
                {
                    return GetPathFromPredecessorGrid(predecessorGrid, sourcePos, endPos);
                }

                q.Enqueue(testPosition);
            }
        }

        return null;
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
        }

        return path;
    }
}