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

    public void SetPath(List<Vector2Int> path)
    {
        bool arePathsSame = true;
        if (this.PathToSource != null && this.PathToSource.Count == path?.Count)
        {
            for (int i = 0; i < path?.Count; i++)
            {
                if (this.PathToSource[i] != path[i])
                {
                    arePathsSame = false;
                    break;
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

        this.PathToSource = path;
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