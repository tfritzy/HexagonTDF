using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConveyorCell : Cell
{
    private const float VELOCITY = .5f;
    private const int SLOTS_ON_BELT = 10;
    private LinkedList<ItemOnBelt> ItemsOnBelt;
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
            if (IsSource)
            {
                return;
            }

            _prev = value;
            ResetPathPoints();
            ConfigureLines(this);
        }
    }
    private List<Vector3> pointsOnPath;
    private float[] pointProgressCache;
    private float totalPathDistance;

    /// <summary>
    /// If true this conveyer can only be the start of a line.
    /// </summary>
    public bool IsSource { get; private set; }

    public class ItemOnBelt
    {
        public InstantiatedItem ItemInst;
        public int CurrentPathPoint;
        public float ProgressAlongPath;
        public float MinBound => ProgressAlongPath - ItemInst.Item.Width;
        public float MaxBound => ProgressAlongPath + ItemInst.Item.Width;
        public bool IsPaused;
    }

    public ConveyorCell(bool isSource)
    {
        this.IsSource = isSource;
    }

    public override void Setup(Character owner)
    {
        base.Setup(owner);
        SetupConveyorDirection();
        ItemsOnBelt = new LinkedList<ItemOnBelt>();
        ResetPathPoints();
    }

    public override void Update()
    {
        MoveItemsForward();
    }

    private void MoveItemsForward()
    {
        if (ItemsOnBelt.Count == 0 || pointsOnPath == null)
        {
            return;
        }

        bool movedOnItem = false;
        LinkedListNode<ItemOnBelt> currentItem = ItemsOnBelt.First;
        while (currentItem != null)
        {
            ItemOnBelt iterRes = currentItem.Value;
            if (iterRes.CurrentPathPoint < pointsOnPath.Count - 1)
            {
                if (!iterRes.IsPaused)
                {
                    float currentProgress = iterRes.ProgressAlongPath;
                    if (currentProgress + iterRes.ItemInst.Item.Width < GetMinBoundOfNextItem(currentItem))
                    {
                        Vector3 deltaToNextPoint =
                            pointsOnPath[iterRes.CurrentPathPoint + 1] -
                            iterRes.ItemInst.gameObject.transform.position;
                        Vector3 moveDelta = deltaToNextPoint.normalized * VELOCITY * Time.deltaTime;
                        iterRes.ItemInst.gameObject.transform.position += moveDelta;
                        iterRes.ItemInst.transform.LookAt(pointsOnPath[iterRes.CurrentPathPoint + 1]);
                        iterRes.ProgressAlongPath += moveDelta.magnitude;

                        if (deltaToNextPoint.magnitude < .02f)
                        {
                            iterRes.CurrentPathPoint += 1;
                        }
                    }
                }
            }
            else
            {
                if (this.Next != null && 
                    this.Next.CanAccept(iterRes.ItemInst.Item.Width))
                {
                    movedOnItem = true;
                    this.Next.AddItem(iterRes.ItemInst);
                }
            }

            currentItem = currentItem.Next;
        }

        if (movedOnItem)
        {
            ItemsOnBelt.RemoveLast();
        }
    }

    public void AddItem(InstantiatedItem item, float progress)
    {
        var newItem = new ItemOnBelt
        {
            ItemInst = item,
            CurrentPathPoint = 0,
            ProgressAlongPath = progress
        };

        RecalculateItemPointOnPath(newItem);

        var currentNode = ItemsOnBelt.First;
        while (currentNode != null && currentNode.Value.ProgressAlongPath < progress)
        {
            currentNode = currentNode.Next;
        }

        if (currentNode != null)
        {
            ItemsOnBelt.AddBefore(currentNode, newItem);
        } else 
        {
            ItemsOnBelt.AddLast(newItem);
        }
    }

    private void RecalculateItemPointOnPath(ItemOnBelt item)
    {
        int i;
        int currentPoint = 0;
        float currentProgress = item.ProgressAlongPath;
        for (i = 1; i < pointsOnPath.Count; i++)
        {
            if (pointProgressCache[i] >= currentProgress && pointProgressCache[i - 1] <= currentProgress)
            {
                float midpointLength = pointProgressCache[i] - currentProgress;
                Vector3 toPoint = pointsOnPath[i] - pointsOnPath[i - 1];
                item.ItemInst.transform.position = pointsOnPath[i - 1] + toPoint.normalized * midpointLength;
                currentPoint = i - 1;
                break;
            }
        }
        
        if (i == pointsOnPath.Count && currentPoint == 0)
        {
            // If we iterated to the end without finding an option, 
            // it means the progress is greater than any actual point.
            item.CurrentPathPoint = pointsOnPath.Count - 1;
            item.ProgressAlongPath = 1f;
        } else
        {
            item.CurrentPathPoint = currentPoint;
            item.ProgressAlongPath = currentProgress;
        }
    }

    public void AddItem(InstantiatedItem item)
    {
        ItemsOnBelt.AddFirst(new ItemOnBelt { ItemInst = item, CurrentPathPoint = 0 });
    }

    public bool CanAccept(float itemWidth)
    {
        if (ItemsOnBelt.Count == 0)
        {
            return true;
        }

        ItemOnBelt firstResource = ItemsOnBelt.First.Value;

        // Items get added with their center at the start of the path,
        // so we need to check if there's enough space there to fit
        // half the item's size.
        return firstResource.MinBound > itemWidth;
    }

    public ItemOnBelt GetFurthestAlongResourceOfType(ItemType ItemType)
    {
        float maxProgress = float.MinValue;
        ItemOnBelt maxResource = null;
        foreach (var resource in ItemsOnBelt)
        {
            if (resource.ItemInst.Item.Type == ItemType && resource.ProgressAlongPath > maxProgress)
            {
                maxProgress = resource.ProgressAlongPath;
                maxResource = resource;
            }
        }

        return maxResource;
    }

    public void RemoveItem(Guid itemId)
    {
        var node = ItemsOnBelt.First;
        while (node != null)
        {
            if (node.Value.ItemInst.Item.Id == itemId)
            {
                ItemsOnBelt.Remove(node);
                return;
            }

            node = node.Next;
        }
    }

    private float GetMinBoundOfNextItem(LinkedListNode<ItemOnBelt> item)
    {
        if (item.Next != null)
        {
            ItemOnBelt resource = item.Next.Value;
            return resource.MinBound;
        }

        if (this.Next != null && this.Next.ItemsOnBelt.First != null)
        {
            ItemOnBelt resource = this.Next.ItemsOnBelt.First.Value;
            return resource.MinBound + totalPathDistance;
        }

        return int.MaxValue;
    }

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
                building.ResourceCollectionCell == null &&
                !building.ConveyorCell.IsSource)
            {
                this.Next = building.ConveyorCell;
                building.ConveyorCell.Prev = this;
            }
        }
    }

    private void ResetPathPoints()
    {
        pointsOnPath = GetPointsOnPath();

        if (pointsOnPath.Count == 0)
        {
            return;
        }

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
            cumulativeProgress += (pointsOnPath[i] - pointsOnPath[i - 1]).magnitude;
            pointProgressCache[i] = cumulativeProgress;
        }

        if (ItemsOnBelt != null && ItemsOnBelt.Count > 0)
        {
            foreach (var item in ItemsOnBelt)
            {
                RecalculateItemPointOnPath(item);
            }

            List<ItemOnBelt> itemList = ItemsOnBelt.ToList();
            itemList.Sort(
                (ItemOnBelt item1, ItemOnBelt item2) => 
                    (int)(item1.ProgressAlongPath * 100) - 
                    (int)(item2.ProgressAlongPath * 100));
            ItemsOnBelt = new LinkedList<ItemOnBelt>(itemList);
        }
    }

    private void ConfigureLines(ConveyorCell conveyor)
    {
        conveyor.Owner.GetComponent<LineRenderer>().positionCount = this.pointsOnPath.Count;
        conveyor.Owner.GetComponent<LineRenderer>().SetPositions(this.pointsOnPath.ToArray());
    }

    private List<Vector3> GetPointsOnPath()
    {
        List<Vector3> points;
        if (Prev != null && Next != null)
        {
            points = new List<Vector3>()
            {
                this.Owner.transform.position + (Prev.Owner.transform.position - this.Owner.transform.position).normalized * Constants.HEXAGON_r,
                this.Owner.transform.position,
                this.Owner.transform.position + (Next.Owner.transform.position - this.Owner.transform.position).normalized * Constants.HEXAGON_r,
            };
        }
        else if (Prev != null && Next == null)
        {
            points = new List<Vector3>()
            {
                this.Owner.transform.position + -(this.Owner.transform.position - Prev.Owner.transform.position).normalized * Constants.HEXAGON_r,
                this.Owner.transform.position,
            };
        }
        else if (Next != null && Prev == null)
        {
            points = new List<Vector3>()
            {
                this.Owner.transform.position,
                this.Owner.transform.position + (Next.Owner.transform.position - this.Owner.transform.position).normalized * Constants.HEXAGON_r,
            };
        }
        else
        {
            points = new List<Vector3>();
        }

        return points;
    }
}