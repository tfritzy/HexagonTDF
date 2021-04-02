using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    public float MovementSpeed;
    public int PathProgress;
    public override Alliances Alliance => Alliances.Illigons;
    public override Alliances Enemies => Alliances.Player;
    public override int StartingHealth => 10;
    public virtual int GoldReward => 1;

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
