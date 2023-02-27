using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConveyorCell : Cell
{

    /// <summary>
    /// If true this conveyer can only be the start of a line.
    /// </summary>
    public bool IsSource { get; private set; }
    public bool IsTermination { get; private set; }
    public ConveyorCell Prev { get; private set; }
    public ConveyorCell Next { get; private set; }
    private const float VELOCITY = .75f;
    private const float ITEM_DIST_ABOVE_GROUND = .38f;
    private ConveyorBody body;

    public Belt ConveyorBelt { get; private set; }

    public class ItemOnBelt
    {
        public InstantiatedItem ItemInst;
        public int CurrentPathPoint;
        public float ProgressAlongPath;
        public float MinBound => ProgressAlongPath - ItemInst.Item.Width;
        public float MaxBound => ProgressAlongPath + ItemInst.Item.Width;
        public bool IsPaused;
    }

    public class Belt
    {
        public LinkedList<ItemOnBelt> Items;
        public float TotalLength { get; private set; }
        public HexSide InputSide;
        public HexSide OutputSide;
        public Transform[] Points => this.conveyorBody.ActivePoints;

        private ConveyorBody conveyorBody;

        public Belt(LinkedList<ItemOnBelt> items, HexSide input, HexSide outputSide, ConveyorBody body)
        {
            this.Items = items;
            this.InputSide = input;
            this.OutputSide = outputSide;
            this.conveyorBody = body;
        }
    }

    public ConveyorCell(bool isSource = false, bool isTermination = false)
    {
        this.IsSource = isSource;
        this.IsTermination = isTermination;
    }

    public override void Setup(Character owner)
    {
        base.Setup(owner);
        this.ConveyorBelt = null;
        this.body = this.Owner.GetComponentInChildren<ConveyorBody>();
        this.body.Setup();
        SetupConveyorDirection();
    }

    public override void Update()
    {
        MoveItemsForward(this.ConveyorBelt, this.Next?.ConveyorBelt);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        Vector2Int ownerPos = this.Owner.GridPosition;
        for (int i = 0; i < 6; i++)
        {
            Vector2Int neighbor = Helpers.GetNeighborPosition(ownerPos.x, ownerPos.y, (HexSide)i);
            Building neighborBuilding = Managers.Board.GetBuilding(neighbor);
            if (neighborBuilding?.ConveyorCell != null)
            {
                neighborBuilding.ConveyorCell.InformOfDeath(this);
            }
        }
    }

    private void InformOfDeath(ConveyorCell cell)
    {
        if (this.Next == cell)
        {
            LinkConveyors(this, null);
        }

        SetupConveyorDirection();
    }

    private void MoveItemsForward(Belt belt, Belt nextBelt)
    {
        if (belt == null || belt.Points.Length == 0)
        {
            return;
        }

        bool movedOnItem = false;
        LinkedListNode<ItemOnBelt> currentItem = belt.Items.First;
        while (currentItem != null)
        {
            ItemOnBelt iterRes = currentItem.Value;

            if (currentItem.Value.ItemInst == null)
            {
                Debug.LogWarning("Removing item from belt that wasn't properly disposed");
                belt.Items.Remove(currentItem);
                currentItem = currentItem.Next;
                continue;
            }

            if (iterRes.CurrentPathPoint < belt.Points.Length - 1)
            {
                if (!iterRes.IsPaused)
                {
                    float currentProgress = iterRes.ProgressAlongPath;
                    if (currentProgress + iterRes.ItemInst.Item.Width < GetMinBoundOfNextItem(currentItem, nextBelt))
                    {
                        Vector3 deltaToNextPoint =
                            belt.Points[iterRes.CurrentPathPoint + 1].position -
                            iterRes.ItemInst.gameObject.transform.position;
                        Vector3 moveDelta = deltaToNextPoint.normalized * VELOCITY * Time.deltaTime;
                        iterRes.ItemInst.gameObject.transform.position += moveDelta;
                        Vector3 targetRot = iterRes.ItemInst.Item.ForwardOnConveyor *
                            (belt.Points[iterRes.CurrentPathPoint + 1].position - iterRes.ItemInst.gameObject.transform.position);
                        iterRes.ItemInst.transform.forward = Vector3.Lerp(iterRes.ItemInst.transform.forward, targetRot, Time.deltaTime * 3);
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
                if (nextBelt != null && CanAccept(nextBelt, iterRes.ItemInst.Item.Width))
                {
                    movedOnItem = true;
                    nextBelt.Items.AddFirst(iterRes);
                    iterRes.CurrentPathPoint = 0;
                    iterRes.ProgressAlongPath = 0f;
                    break;
                }
            }

            currentItem = currentItem.Next;
        }

        if (movedOnItem)
        {
            belt.Items.RemoveLast();
        }
    }

    public void AddItem(HexSide fromSide, InstantiatedItem inst)
    {
        AddItem(this.ConveyorBelt, inst);
    }

    public void AddItem(Belt toBelt, InstantiatedItem inst)
    {
        var newItem = new ItemOnBelt
        {
            ItemInst = inst,
            CurrentPathPoint = 0,
            ProgressAlongPath = 0f,
        };

        var firstItem = toBelt.Items.First;
        if (firstItem != null && firstItem.Value.MinBound < inst.Item.Width)
        {
            throw new System.Exception("Tried to add an item to the belt which overlaps with an existing item.");
        }

        if (inst.TryGetComponent<Collider>(out Collider col))
        {
            col.enabled = false;
        }
        toBelt.Items.AddFirst(newItem);
    }

    public bool CanAccept(HexSide side, float itemWidth)
    {
        return CanAccept(this.ConveyorBelt, itemWidth);
    }

    public bool CanAccept(Belt belt, float itemWidth)
    {
        if (belt?.Items == null)
        {
            return false;
        }

        if (belt.Items.Count == 0)
        {
            return true;
        }

        ItemOnBelt firstResource = belt.Items.First.Value;

        // Items get added with their center at the start of the path,
        // so we need to check if there's enough space there to fit
        // half the item's size.
        return firstResource.MinBound > itemWidth;
    }

    public ItemOnBelt GetFurthestAlongResourceOfType(Belt belt, ItemType ItemType)
    {
        float maxProgress = float.MinValue;
        ItemOnBelt maxResource = null;
        foreach (var resource in belt.Items)
        {
            if (resource.ItemInst.Item.Type == ItemType && resource.ProgressAlongPath > maxProgress)
            {
                maxProgress = resource.ProgressAlongPath;
                maxResource = resource;
            }
        }

        return maxResource;
    }

    public ItemOnBelt GetFurthestAlongResource(Belt belt)
    {
        return belt.Items.Last?.Value;
    }

    public void RemoveItem(Belt belt, Guid itemId)
    {
        var node = belt.Items.First;
        while (node != null)
        {
            if (node.Value.ItemInst.Item.Id == itemId)
            {
                belt.Items.Remove(node);
                return;
            }

            node = node.Next;
        }

        throw new System.Exception($"Tried to remove item with id {itemId} from this belt, but it wasn't there.");
    }

    public void SwitchOutput(ConveyorCell newOutput)
    {
        bool isNewANeighbor = false;
        for (int i = 0; i < 6; i++)
        {
            Vector2Int neighbor = Helpers.GetNeighborPosition(this.Owner.GridPosition, (HexSide)i);
            if (Managers.Board.GetBuilding(neighbor)?.ConveyorCell == newOutput)
            {
                isNewANeighbor = true;
                break;
            }
        }

        if (isNewANeighbor)
        {
            var currentItems = this.ConveyorBelt?.Items ?? new LinkedList<ItemOnBelt>();
            LinkConveyors(this, newOutput);
            this.ConveyorBelt.Items = currentItems;
        }
    }

    private float GetMinBoundOfNextItem(LinkedListNode<ItemOnBelt> item, Belt nextBelt)
    {
        if (item.Next != null)
        {
            ItemOnBelt resource = item.Next.Value;
            return resource.MinBound;
        }

        if (nextBelt?.Items.First != null)
        {
            ItemOnBelt resource = nextBelt.Items.First.Value;
            return resource.MinBound + nextBelt.TotalLength;
        }

        return int.MaxValue;
    }

    private void SetupConveyorDirection()
    {
        for (int i = 0; i < 6; i++)
        {
            Vector2Int neighbor = Helpers.GetNeighborPosition(this.Owner.GridPosition, (HexSide)i);
            Building building = Managers.Board.GetBuilding(neighbor);

            if (CanBePrev(building))
            {
                LinkConveyors(building.ConveyorCell, this);
            }

            if (CanBeNext(building))
            {
                LinkConveyors(this, building.ConveyorCell);
            }
        }
    }

    private bool CanBePrev(Building building)
    {
        if (this.Prev != null)
        {
            return false;
        }

        if (building?.ConveyorCell == null)
        {
            return false;
        }

        if (building.ConveyorCell.Next != null)
        {
            return false;
        }

        if (building.ConveyorCell.IsTermination)
        {
            return false;
        }

        if (building.ConveyorCell.Next != null)
        {
            return false;
        }

        if (IsInputTooSharp(building))
        {
            return false;
        }

        return true;
    }

    private bool IsInputTooSharp(Building building)
    {
        HexSide? outputSide = this.ConveyorBelt?.OutputSide;
        HexSide checkInputSide =
                CalculateHexSide(
                    this.Owner.transform.position,
                    building.transform.position);
        if (outputSide != null)
        {
            int angle = Math.Abs((int)checkInputSide - (int)outputSide);

            // Can't do 60 degree turns.
            if (angle < 2)
            {
                return true;
            }
        }

        return false;
    }

    private bool CanBeNext(Building building)
    {
        if (this.Next != null)
        {
            return false;
        }

        if (this.Next != null)
        {
            return false;
        }

        if (building?.ConveyorCell == null)
        {
            return false;
        }

        if (building.ConveyorCell.Next == this)
        {
            return false;
        }

        if (building.ConveyorCell.IsSource)
        {
            return false;
        }

        if (building.ConveyorCell.Prev != null)
        {
            return false;
        }

        if (building.ConveyorCell.IsInputTooSharp((Building)this.Owner))
        {
            return false;
        }

        return true;
    }

    private void LinkConveyors(ConveyorCell source, ConveyorCell target)
    {
        if (target == null)
        {
            source.Next = null;
            source.body.Setup();
            return;
        }

        HexSide sourceOutputSide = CalculateHexSide(source.Owner.transform.position, target.Owner.transform.position);
        HexSide targetInputSide = GetOppositeSide(sourceOutputSide);
        HexSide sourceInputSide =
            source.Prev != null ?
                CalculateHexSide(source.Prev.Owner.transform.position, source.Owner.transform.position) :
                targetInputSide;

        var sourceItems = source.ConveyorBelt?.Items ?? new LinkedList<ItemOnBelt>();
        source.ConveyorBelt = new Belt(sourceItems, sourceInputSide, sourceOutputSide, source.body);

        var targetItems = target.ConveyorBelt?.Items ?? new LinkedList<ItemOnBelt>();
        HexSide targetOutputSide = target.ConveyorBelt?.OutputSide ?? sourceOutputSide;
        target.ConveyorBelt = new Belt(targetItems, targetInputSide, targetOutputSide, target.body);

        source.Next = target;
        target.Prev = source;

        source.body.Setup();
        target.body.Setup();
    }

    private static List<Vector3> GetPathForAngle(float angle, bool reversed = false)
    {
        // The world is setup so that a hexagon's north is in the positive x direction.
        angle += 90;

        List<Vector3> points = new List<Vector3>
        {
            new Vector3(
                    Mathf.Cos(angle * Mathf.Deg2Rad),
                    0,
                    Mathf.Sin(angle * Mathf.Deg2Rad)) * Constants.HEXAGON_r,
            Vector3.zero,
        };

        if (reversed)
        {
            points.Reverse();
        }

        return points;
    }

    private static List<Vector3> GetPointsForSide(HexSide side)
    {
        switch (side)
        {
            case (HexSide.North):
                return new List<Vector3> { Vector3.up * ITEM_DIST_ABOVE_GROUND, new Vector3(0, ITEM_DIST_ABOVE_GROUND, Constants.HEXAGON_r) };
            case (HexSide.NorthEast):
                return new List<Vector3> { Vector3.up * ITEM_DIST_ABOVE_GROUND, new Vector3(.75f, ITEM_DIST_ABOVE_GROUND, .43f) };
            case (HexSide.SouthEast):
                return new List<Vector3> { Vector3.up * ITEM_DIST_ABOVE_GROUND, new Vector3(.75f, ITEM_DIST_ABOVE_GROUND, -.43f) };
            case (HexSide.South):
                return new List<Vector3> { Vector3.up * ITEM_DIST_ABOVE_GROUND, new Vector3(0, ITEM_DIST_ABOVE_GROUND, -Constants.HEXAGON_r) };
            case (HexSide.SouthWest):
                return new List<Vector3> { Vector3.up * ITEM_DIST_ABOVE_GROUND, new Vector3(-.75f, ITEM_DIST_ABOVE_GROUND, -.43f) };
            case (HexSide.NorthWest):
                return new List<Vector3> { Vector3.up * ITEM_DIST_ABOVE_GROUND, new Vector3(-.75f, ITEM_DIST_ABOVE_GROUND, .43f) };
            default: throw new Exception("Unknown side: " + side);
        }
    }

    private static HexSide CalculateHexSide(Vector3 sourcePos, Vector3 targetPos)
    {
        Vector3 delta = targetPos - sourcePos;
        delta.y = 0;
        float angle = Vector3.Angle(Vector3.forward, delta);
        if (targetPos.x > sourcePos.x)
        {
            if (angle < 30)
                return HexSide.North;
            else if (angle < 90)
                return HexSide.NorthEast;
            else if (angle < 150)
                return HexSide.SouthEast;
            else
                return HexSide.South;
        }
        else
        {
            if (angle < 30)
                return HexSide.North;
            else if (angle < 90)
                return HexSide.NorthWest;
            else if (angle < 150)
                return HexSide.SouthWest;
            else
                return HexSide.South;
        }
    }

    private HexSide GetOppositeSide(HexSide side)
    {
        switch (side)
        {
            case (HexSide.North):
                return HexSide.South;
            case (HexSide.NorthEast):
                return HexSide.SouthWest;
            case (HexSide.SouthEast):
                return HexSide.NorthWest;
            case (HexSide.South):
                return HexSide.North;
            case (HexSide.SouthWest):
                return HexSide.NorthEast;
            case (HexSide.NorthWest):
                return HexSide.SouthEast;
            default:
                throw new Exception("Unknown side: " + side);
        }
    }
}