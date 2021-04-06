using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Enemy : Character
{
    public int PathProgress;
    public override Alliances Alliance => Alliances.Illigons;
    public override Alliances Enemies => Alliances.Player;
    public abstract int GoldReward { get; }
    public abstract float MovementSpeed { get; }
    public abstract EnemyType Type { get; }

    private bool isDead;
    private Rigidbody rb;
    private Portal portal;
    private Guid pathId;
    private List<Vector2Int> path;

    public override float Power
    {
        get { return ((float)StartingHealth / 2.5f) + (MovementSpeed - 1); }
    }

    public void SetPortal(Portal portal)
    {
        this.portal = portal;
        this.pathId = portal.PathId;
        this.path = portal.PathToSource;
    }

    protected override void Setup()
    {
        PathProgress = 0;
        this.rb = GetComponent<Rigidbody>();
        base.Setup();
    }

    protected override void UpdateLoop()
    {
        if (isDead)
        {
            return;
        }

        FollowPath();
        base.UpdateLoop();
    }

    protected override void Die()
    {
        Managers.ResourceStore.Add(ResourceType.Gold, GoldReward);
        base.Die();
    }

    private void FollowPath()
    {
        if (portal.PathId != this.pathId)
        {
            RecalculatePath();
        }

        if (PathProgress >= path.Count)
        {
            Managers.Map.Source.TakeDamage(1);
            Destroy(this.gameObject);
            isDead = true;
            return;
        }

        Vector3 difference = (Hexagon.ToWorldPosition(path[PathProgress]) - this.transform.position);
        difference.y = 0;
        this.rb.velocity = difference.normalized * MovementSpeed;

        if (difference.magnitude < .1f)
        {
            PathProgress += 1;
        }
    }

    private void RecalculatePath()
    {
        List<Vector2Int> pathToSource = Helpers.FindPath(Managers.Map.Hexagons, this.path[PathProgress], Managers.Map.Source.Position);
        this.PathProgress = 0;
        this.pathId = portal.PathId;
        this.path = pathToSource;
    }
}
