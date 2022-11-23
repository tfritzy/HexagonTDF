using System.Collections.Generic;
using UnityEngine;

public class ConveyorCell : Cell
{
    private const float VELOCITY = .3f;
    private float CurrentVelocity;
    private const int SLOTS_ON_BELT = 10;
    private LinkedList<ResourceOnBelt> ItemsOnBelt;
    private ConveyorCell _next;
    public ConveyorCell Next
    {
        get { return _next; }
        set
        {
            _next = value;
            ResetPathPoints();
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
            ResetPathPoints();
            ConfigureLines(this);
        }
    }
    private List<Vector3> pointsOnPath;
    private float[] pointProgressCache;
    private float totalPathDistance;

    private class ResourceOnBelt
    {
        public Resource Resource;
        public int CurrentPathPoint;
        public bool Blocked;
    }

    public override void Setup(Character owner)
    {
        base.Setup(owner);
        this.CurrentVelocity = VELOCITY;

        SetupConveyorDirection();

        ItemsOnBelt = new LinkedList<ResourceOnBelt>();
    }

    public override void Update()
    {
        MoveItemsForward();
    }

    private void MoveItemsForward()
    {
        if (ItemsOnBelt.Count == 0)
        {
            return;
        }

        bool movedOnItem = false;
        LinkedListNode<ResourceOnBelt> currentItem = ItemsOnBelt.First;
        while (currentItem != null)
        {
            ResourceOnBelt iterRes = currentItem.Value;
            if (iterRes.CurrentPathPoint < pointsOnPath.Count - 1)
            {
                float currentProgress = getProgressOfResource(iterRes);
                if (currentProgress + iterRes.Resource.Width < GetMinBoundOfNextItem(currentItem))
                {
                    Vector3 delta =
                        pointsOnPath[iterRes.CurrentPathPoint + 1] -
                        iterRes.Resource.gameObject.transform.position;
                    iterRes.Resource.gameObject.transform.position += delta.normalized * this.CurrentVelocity * Time.deltaTime;

                    if (delta.magnitude < .05f)
                    {
                        iterRes.CurrentPathPoint += 1;
                    }
                }
            }
            else
            {
                if (this.Next != null && this.Next.CanAcceptItem())
                {
                    movedOnItem = true;
                    this.Next.AddItem(iterRes.Resource);
                }
            }

            currentItem = currentItem.Next;
        }

        if (movedOnItem)
        {
            ItemsOnBelt.RemoveLast();
        }
    }

    public void AddItem(Resource resource)
    {
        ItemsOnBelt.AddFirst(new ResourceOnBelt { Resource = resource, CurrentPathPoint = 0 });
    }

    public bool CanAcceptItem()
    {
        if (ItemsOnBelt.Count == 0)
        {
            return true;
        }

        ResourceOnBelt firstResource = ItemsOnBelt.First.Value;
        float progress = getProgressOfResource(firstResource);
        float minBound = progress - firstResource.Resource.Width;

        return minBound > 0;
    }

    private float GetMinBoundOfNextItem(LinkedListNode<ResourceOnBelt> item)
    {
        if (item.Next != null)
        {
            ResourceOnBelt resource = item.Next.Value;
            return getProgressOfResource(resource) - resource.Resource.Width;
        }

        if (this.Next.ItemsOnBelt.First != null)
        {
            ResourceOnBelt resource = this.Next.ItemsOnBelt.First.Value;
            return this.Next.getProgressOfResource(resource) -
                   resource.Resource.Width +
                   1f;
        }

        return int.MinValue;
    }

    private float getProgressOfResource(ResourceOnBelt resource)
    {
        return pointProgressCache[resource.CurrentPathPoint] +
                (resource.Resource.transform.position - pointsOnPath[resource.CurrentPathPoint])
                    .magnitude / totalPathDistance;
    }

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

    private void ResetPathPoints()
    {
        pointsOnPath = GetPointsOnPath();

        totalPathDistance = 0f;
        for (int i = 1; i < pointsOnPath.Count; i++)
        {
            totalPathDistance += (pointsOnPath[i] - pointsOnPath[i - 1]).magnitude;
        }

        pointProgressCache = new float[pointsOnPath.Count];
        pointProgressCache[0] = 0;
        float cumulativeProgress = 0f;
        for (int i = 1; i < pointsOnPath.Count; i++)
        {
            cumulativeProgress += (pointsOnPath[i] - pointsOnPath[i - 1]).magnitude / totalPathDistance;
            pointProgressCache[i] = cumulativeProgress;
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