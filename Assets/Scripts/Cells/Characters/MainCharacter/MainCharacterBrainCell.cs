using System.Collections.Generic;
using UnityEngine;

public class MainCharacterBrainCell : BrainCell
{
    private LinkedList<Vector2Int> pathToFollow;
    private const float MovementSpeed = 4f;

    public MainCharacterBrainCell()
    {

    }

    public void SetPath(LinkedList<Vector2Int> path)
    {
        Debug.Log(string.Join(", ", path));
        this.pathToFollow = path;
    }

    public override void Update()
    {
        if (pathToFollow != null && pathToFollow.Count > 0)
        {
            Vector2Int nextPos = pathToFollow.First.Value;
            Vector3 delta = Helpers.ToWorldPosition(nextPos) - this.Owner.transform.position;

            if (delta.magnitude < .2f)
            {
                pathToFollow.RemoveFirst();
            }

            this.Owner.transform.position += delta.normalized * Time.deltaTime * MovementSpeed;
        }
    }
}