using System;
using System.Collections.Generic;
using UnityEngine;

public class ShoreMono : HexagonMono
{
    public Guid PathId;
    public List<Vector2Int> PathToSource;
    private LineRenderer lineRenderer;
    private GameObject icon;

    protected override void Setup()
    {
        base.Setup();
        this.lineRenderer = this.GetComponent<LineRenderer>();
    }

    public void RecalculatePath()
    {
        List<Vector2Int> oldPath = PathToSource;
        PathToSource = Helpers.FindPath(Managers.Board.Map, GridPosition, Managers.Board.Source.Position, Helpers.IsTraversable);
        Debug.Log("Shore calculated path of length: " + PathToSource.Count);
        if (PathToSource == null)
        {
            throw new System.NullReferenceException($"Shore was unable to find path to source.");
        }

        bool arePathsSame = true;
        if (oldPath != null && oldPath.Count == PathToSource.Count)
        {
            for (int i = 0; i < PathToSource.Count; i++)
            {
                if (PathToSource[i] != oldPath[i])
                {
                    arePathsSame = false;
                }
            }
        }
        else
        {
            arePathsSame = false;
        }

        if (arePathsSame == false)
        {

            PathId = Guid.NewGuid();
            ResetLineRenderer();
        }
    }

    public void SetIconColor(Color color)
    {
        if (this.icon == null)
        {
            this.icon = transform.Find("Icon").gameObject;
        }

        this.icon.GetComponent<MeshRenderer>().material.SetColor("_Color", color);
    }

    private void ResetLineRenderer()
    {
        // TODO: Show path sometimes
        return;

        lineRenderer.positionCount = PathToSource.Count + 1;
        lineRenderer.SetPosition(0, this.transform.position);
        int i = 0;
        for (i = 1; i < lineRenderer.positionCount; i++)
        {
            Vector2Int pos = PathToSource[i - 1];
            lineRenderer.SetPosition(i, Managers.Board.Hexagons[pos.x, pos.y].transform.position + Vector3.up * .01f);
        }
    }
}