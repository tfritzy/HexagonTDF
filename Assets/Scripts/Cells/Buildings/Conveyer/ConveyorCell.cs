using System.Collections.Generic;
using UnityEngine;

public class ConveyorCell : Cell
{
    private const float VELOCITY = .2f;
    private const int SLOTS_ON_BELT = 10;
    public LinkedList<Resource> ItemsOnBelt;
    private ConveyorCell _next;
    public ConveyorCell Next
    {
        get { return _next; }
        set {
            _next = value;
            pointsOnPath = GetPointsOnPath();
            ConfigureLines(this);
        }
    }
    private ConveyorCell _prev;
    public ConveyorCell Prev
    {
        get { return _prev; }
        set
        {
            _prev = value;
            pointsOnPath = GetPointsOnPath();
            ConfigureLines(this);
        }
    }
    private List<Vector3> pointsOnPath;

    public override void Setup(Character owner)
    {
        base.Setup(owner);

        SetupConveyorDirection();

        ItemsOnBelt = new LinkedList<Resource>();
    }

    public override void Update()
    {
        MoveItemsForward();
    }

    private void MoveItemsForward()
    {
        // bool movedOnItem = false;
        // foreach (Resource resource in ItemsOnBelt)
        // {
        //     resource.transform.position += Direction * VELOCITY * Time.deltaTime;

        //     float progress = getProgressAlongRoute(resource);
        //     if (progress > 1f)
        //     {
        //         if (this.Output != null && this.Output.CanAcceptItem())
        //         {
        //             this.Output.AddItem(resource);
        //             Direction = Vector3.right;
        //             movedOnItem = true;
        //         }
        //         else
        //         {
        //             Direction = Vector3.zero;
        //         }
        //     }
        // }

        // if (movedOnItem)
        // {
        //     ItemsOnBelt.RemoveLast();
        // }
    }

    public void AddItem(Resource resource)
    {
        ItemsOnBelt.AddFirst(resource);
    }

    // public bool CanAcceptItem()
    // {
    //     if (ItemsOnBelt.Count == 0)
    //     {
    //         return true;
    //     }

    //     Resource firstResource = ItemsOnBelt.Last.Value;
    //     float progress = getProgressAlongRoute(firstResource);
    //     int minBound = (int)(progress * 100) - firstResource.Width / 2;

    //     return minBound > 0;
    // }

    // private float getProgressAlongRoute(Resource resource)
    // {

    // }

    private void SetupConveyorDirection()
    {
        for (int i = 0; i < 6; i++)
        {
            Vector2Int neighbor = Helpers.GetNeighborPosition(this.Owner.GridPosition, i);
            Building building = Managers.Board.GetBuilding(neighbor);

            if (building == null || building.ConveyorCell == null)
            {
                continue;
            }

            if (building.ConveyorCell.Next == null)
            {
                this.Prev = building.ConveyorCell;
                building.ConveyorCell.Next = this;
            }

            if (building.ConveyorCell.Prev == null &&
                building.ConveyorCell.Next != this &&
                building.ResourceCollectionCell == null)
            {
                this.Next = building.ConveyorCell;
                building.ConveyorCell.Prev = this;
            }
        }
    }

    private void ConfigureLines(ConveyorCell conveyor)
    {
        conveyor.Owner.GetComponent<LineRenderer>().positionCount = this.pointsOnPath.Count;
        conveyor.Owner.GetComponent<LineRenderer>().SetPositions(this.pointsOnPath.ToArray());
    }

    private List<Vector3> GetPointsOnPath()
    {
        if (Prev != null && Next != null)
        {
            return new List<Vector3>()
            {
                this.Owner.transform.position + (Prev.Owner.transform.position - this.Owner.transform.position).normalized * Constants.HEXAGON_r,
                this.Owner.transform.position,
                this.Owner.transform.position + (Next.Owner.transform.position - this.Owner.transform.position).normalized * Constants.HEXAGON_r,
            };
        }
        else if (Prev != null && Next == null)
        {
            return new List<Vector3>()
            {
                this.Owner.transform.position + -(this.Owner.transform.position - Prev.Owner.transform.position).normalized * Constants.HEXAGON_r,
                this.Owner.transform.position,
                this.Owner.transform.position + (this.Owner.transform.position - Prev.Owner.transform.position).normalized * Constants.HEXAGON_r,
            };
        }
        else if (Next != null && Prev == null)
        {
            return new List<Vector3>()
            {
                this.Owner.transform.position + -(Next.Owner.transform.position - this.Owner.transform.position).normalized * Constants.HEXAGON_r,
                this.Owner.transform.position,
                this.Owner.transform.position + (Next.Owner.transform.position - this.Owner.transform.position).normalized * Constants.HEXAGON_r,
            };
        }
        else
        {
            return new List<Vector3>();
        }
    }
}