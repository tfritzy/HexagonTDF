using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    public virtual float MovementSpeed => 1;
    public int PathProgress;
    public override Alliances Alliance => Alliances.Illigons;
    public override Alliances Enemies => Alliances.Player;
    public override int StartingHealth => 5;
    public virtual int GoldReward => 1;

    private bool isDead;
    private List<Vector2Int> path;
    private Rigidbody rb;

    public void SetPath(List<Vector2Int> path)
    {
        this.path = path;

        if (this.path == null || this.path.Count == 0)
        {
            Destroy(this.gameObject);
            throw new System.ArgumentException("No path was provided to this enemy. Deleting.");
        }
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
        if (PathProgress >= path.Count)
        {
            Managers.Map.Source.TakeDamage(1);
            Destroy(this.gameObject);
            isDead = true;
            return;
        }

        Vector3 difference = (Hexagon.ToWorldPosition(path[PathProgress]) - this.transform.position);
        this.rb.velocity = difference.normalized * MovementSpeed;

        if (difference.magnitude < .1f)
        {
            PathProgress += 1;
        }
    }
}
