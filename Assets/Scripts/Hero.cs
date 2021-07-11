﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Hero : Unit, Interactable
{
    protected List<Vector2Int> PathToTargetPosition;
    public bool IsListeningForTargetPosition;
    public bool IsGuardingHex { get; private set; }
    private int pathProgress;
    private GameObject selectedRing;
    private LineRenderer pathRenderer;

    protected override void Setup()
    {
        base.Setup();
        PathToTargetPosition = new List<Vector2Int>();
        selectedRing = Instantiate(
            Managers.Prefabs.UnitSelectedRing,
            this.transform.position,
            Managers.Prefabs.UnitSelectedRing.transform.rotation,
            this.transform);
        selectedRing.SetActive(false);

        pathRenderer = Instantiate(
            Managers.Prefabs.PathRenderer,
            this.transform.position,
            Managers.Prefabs.PathRenderer.transform.rotation,
            this.transform).GetComponent<LineRenderer>();
        pathRenderer.gameObject.SetActive(false);
    }

    protected override void UpdateLoop()
    {
        base.UpdateLoop();
    }

    private float lastSearchTime;
    protected override void FindTargetCharacter()
    {
        if (!IsGuardingHex)
        {
            return;
        }

        if (this.TargetCharacter == null && Time.time > lastSearchTime + .5f)
        {
            Enemy enemy = SearchForEnemyOnGuardedHex();

            if (enemy != null)
            {
                enemy.EngageInFight(this);
                this.TargetCharacter = enemy;
            }
        }
    }

    protected override bool IsInRangeOfTarget()
    {
        return IsGuardingHex && this.TargetCharacter != null;
    }

    private Enemy SearchForEnemyOnGuardedHex()
    {
        Collider[] hits = Physics.OverlapSphere(
            this.transform.position,
            Constants.HEXAGON_R,
            Constants.Layers.Characters,
            QueryTriggerInteraction.Collide);
        lastSearchTime = Time.time;

        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent<Enemy>(out Enemy enemy))
            {
                return enemy;
            }
        }

        return null;
    }

    public void Interact()
    {
        if (IsListeningForTargetPosition == false)
        {
            IsListeningForTargetPosition = true;
            selectedRing.SetActive(true);
            // TODO: Show user this is listening for target position.
        }
        else
        {
            IsListeningForTargetPosition = false;
            selectedRing.SetActive(false);
            // TODO: Remove elements showing it's looking for target position.
        }
    }

    public void InformHexWasClicked(HexagonMono hex)
    {
        selectedRing.SetActive(false);
        if (IsListeningForTargetPosition)
        {
            FindPath(hex.GridPosition);
            IsListeningForTargetPosition = false;
        }
    }

    protected override void CalculateNextPathingPosition(Vector2Int currentPosition)
    {
        this.pathProgress += 1;

        if (this.pathProgress >= PathToTargetPosition.Count)
        {
            this.Waypoint = null;
            IsGuardingHex = true;
            this.pathRenderer.gameObject.SetActive(false);
            return;
        }

        this.Waypoint = new Waypoint(PathToTargetPosition[pathProgress - 1], PathToTargetPosition[pathProgress]);
    }

    protected override void RecalculatePath()
    {
        if (this.PathToTargetPosition?.Count == 0)
        {
            return;
        }

        FindPath(this.PathToTargetPosition.Last());
    }

    private void FindPath(Vector2Int targetPos)
    {
        pathProgress = 0;
        this.PathId = Managers.Board.PathingId;
        this.PathToTargetPosition = Helpers.FindPathByWalking(
            Managers.Board.Map,
            this.GridPosition,
            targetPos);

        if (this.PathToTargetPosition == null)
        {
            return;
        }

        this.pathRenderer.gameObject.SetActive(true);
        RenderPath();
        if (this.TargetCharacter != null && this.TargetCharacter is Enemy)
        {
            ((Enemy)this.TargetCharacter).DisengageFromFight();
        }

        this.TargetCharacter = null;
        IsGuardingHex = false;
        this.Waypoint = new Waypoint(PathToTargetPosition[0], PathToTargetPosition[1]);
    }

    protected override bool ShouldRecalculatePath()
    {
        return this.PathId != Managers.Board.PathingId;
    }

    private void RenderPath()
    {
        this.pathRenderer.positionCount = this.PathToTargetPosition.Count;
        for (int i = 0; i < this.PathToTargetPosition.Count; i++)
        {
            this.pathRenderer.SetPosition(i, Helpers.ToWorldPosition(this.PathToTargetPosition[i]));
        }
    }
}
