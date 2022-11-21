using System.Collections.Generic;
using UnityEngine;

public class ConveyorCell : Cell
{
    private const float VELOCITY = .2f;
    private const int SLOTS_ON_BELT = 10;
    public LinkedList<Resource> ItemsOnBelt;
    private ConveyorCell Output;
    public Vector3 Direction = Vector3.right;

    public override void Setup(Character owner)
    {
        base.Setup(owner);

        ItemsOnBelt = new LinkedList<Resource>();
    }

    public override void Update()
    {
        LookForOutputHex();
        MoveItemsForward();
    }

    private void MoveItemsForward()
    {
        bool movedOnItem = false;
        foreach (Resource resource in ItemsOnBelt)
        {
            resource.transform.position += Direction * VELOCITY * Time.deltaTime;

            float progress = getProgressAlongRoute(resource);
            if (progress > 1f)
            {
                if (this.Output != null && this.Output.CanAcceptItem())
                {
                    this.Output.AddItem(resource);
                    Direction = Vector3.right;
                    movedOnItem = true;
                } else 
                {
                    Direction = Vector3.zero;
                }
            }
        }

        if (movedOnItem)
        {
            ItemsOnBelt.RemoveLast();
        }
    }
    
    public void AddItem(Resource resource)
    {
        ItemsOnBelt.AddFirst(resource);
    }

    public bool CanAcceptItem()
    {
        if (ItemsOnBelt.Count == 0)
        {
            return true;
        }

        Resource firstResource = ItemsOnBelt.Last.Value;
        float progress = getProgressAlongRoute(firstResource);
        int minBound = (int)(progress * 100) - firstResource.Width / 2;
        
        return minBound > 0;
    }

    private float getProgressAlongRoute(Resource resource)
    {
        Vector3 startPos = Owner.transform.position - Direction * Constants.HEXAGON_r;
        Vector3 terminalPos = Owner.transform.position + Direction * Constants.HEXAGON_r;
        Vector3 fullPath = terminalPos - startPos;
        return (terminalPos - resource.transform.position).magnitude / fullPath.magnitude;
    }

    private float lastOutputCheckTime;
    private void LookForOutputHex()
    {
        if (Time.time < lastOutputCheckTime + .5f)
        {
            return;
        }
        lastOutputCheckTime = Time.time;

        for (int i = 0; i < 6; i++)
        {
            Vector2Int neighbor = Helpers.GetNeighborPosition(this.Owner.GridPosition, i);
            Building building = Managers.Board.GetBuilding(neighbor);
            if (building != null && building.ConveyorCell != null)
            {
                this.Output = building.ConveyorCell;
                break;
            }
        }
    }
}