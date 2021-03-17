using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    public float MovementSpeed;
    public int PathProgress;
    public override Alliances Alliance => Alliances.Illigons;
    public override Alliances Enemies => Alliances.Player;

    private List<Vector2Int> path;
    private Rigidbody rb;


    public void Setup(List<Vector2Int> path)
    {
        this.path = path;
        PathProgress = 0;
        this.rb = GetComponent<Rigidbody>();
    }

    void Start()
    {

    }

    void Update()
    {
        FollowPath();
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
