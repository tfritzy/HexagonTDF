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
    public bool IsMultiInput { get; private set; }
    public ConveyorCell Next { get; private set; }
    private const float VELOCITY = .75f;
    private const float ITEM_DIST_ABOVE_GROUND = .38f;
    private ConveyorBody body;


    // A map of side of a hex to a list of items moving along that path.
    public Dictionary<HexSide, Belt> InputBelts { get; private set; }

    // The items on the way out.
    public Belt OutputBelt { get; private set; }

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
        public List<Vector3> Points;
        public float TotalLength { get; private set; }
        public HexSide Side;

        public Belt(LinkedList<ItemOnBelt> items, List<Vector3> points, HexSide side)
        {
            this.Items = items;
            this.Points = points;
            this.Side = side;

            float totalLength = 0f;
            for (int i = 1; i < points.Count; i++)
            {
                totalLength += (points[i] - points[i - 1]).magnitude;
            }
            this.TotalLength = totalLength;
        }
    }

    public ConveyorCell(bool isSource = false, bool isTermination = false, bool acceptsMultipleInputs = false)
    {
        this.IsSource = isSource;
        this.IsTermination = isTermination;
        this.IsMultiInput = acceptsMultipleInputs;
    }

    public override void Setup(Character owner)
    {
        base.Setup(owner);
        InputBelts = new Dictionary<HexSide, Belt>();
        OutputBelt = null;
        this.body = this.Owner.GetComponent<ConveyorBody>();
        this.body.Setup();
        SetupConveyorDirection();
    }

    public override void Update()
    {
        foreach (HexSide side in InputBelts.Keys)
        {
            MoveItemsForward(InputBelts[side], OutputBelt);
        }

        Belt nextPath = null;
        this.Next?.InputBelts.TryGetValue(this.GetOppositeSide(OutputBelt.Side), out nextPath);
        MoveItemsForward(OutputBelt, nextPath);
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

        RecalculateInputs();
    }

    private void MoveItemsForward(Belt belt, Belt nextBelt)
    {
        if (belt == null || belt.Points.Count == 0)
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

            if (iterRes.CurrentPathPoint < belt.Points.Count - 1)
            {
                if (!iterRes.IsPaused)
                {
                    float currentProgress = iterRes.ProgressAlongPath;
                    if (currentProgress + iterRes.ItemInst.Item.Width < GetMinBoundOfNextItem(currentItem, nextBelt))
                    {
                        Vector3 deltaToNextPoint =
                            belt.Points[iterRes.CurrentPathPoint + 1] -
                            iterRes.ItemInst.gameObject.transform.position;
                        Vector3 moveDelta = deltaToNextPoint.normalized * VELOCITY * Time.deltaTime;
                        iterRes.ItemInst.gameObject.transform.position += moveDelta;
                        iterRes.ItemInst.transform.forward =
                            iterRes.ItemInst.Item.ForwardOnConveyor *
                            (belt.Points[iterRes.CurrentPathPoint + 1] - iterRes.ItemInst.gameObject.transform.position);
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
        if (!InputBelts.ContainsKey(fromSide))
        {
            throw new System.Exception("Tried to add an item on a side which is not configured as an input side.");
        }

        AddItem(InputBelts[fromSide], inst);
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
        if (!InputBelts.ContainsKey(side))
        {
            return false;
        }

        return CanAccept(InputBelts[side], itemWidth);
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
            var currentItems = this.OutputBelt?.Items ?? new LinkedList<ItemOnBelt>();
            LinkConveyors(this, newOutput);
            this.OutputBelt.Items = currentItems;
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

            if (building == null || building.ConveyorCell == null)
            {
                continue;
            }

            if (building.ConveyorCell.Next == null &&
                !building.ConveyorCell.IsTermination &&
                (this.IsMultiInput || this.InputBelts.Count == 0))
            {
                LinkConveyors(building.ConveyorCell, this);
            }

            if (building.ConveyorCell.Next != this &&
                !building.ConveyorCell.IsSource &&
                (building.ConveyorCell.IsMultiInput || building.ConveyorCell.InputBelts.Count == 0))
            {
                LinkConveyors(this, building.ConveyorCell);
            }
        }
    }

    private void LinkConveyors(ConveyorCell source, ConveyorCell target)
    {
        if (target == null)
        {
            source.OutputBelt = null;
            source.Next = null;
            source.body.Setup();
            return;
        }

        if (target.Next == source)
        {
            target.Next = null;
            target.OutputBelt = null;
        }

        HexSide sourceOutputSide = CalculateHexSide(source.Owner.transform.position, target.Owner.transform.position);
        HexSide targetInputSide = GetOppositeSide(sourceOutputSide);

        // todo: recalculate positions of existing items on output belt.
        source.OutputBelt = new Belt(
            new LinkedList<ItemOnBelt>(),
            GetOutputPath(sourceOutputSide, source.Owner.transform.position),
            sourceOutputSide);
        source.Next = target;

        source.RecalculateInputs();
        target.RecalculateInputs();

        source.body.Setup();
        target.body.Setup();
    }

    private void RecalculateInputs()
    {
        if (this.InputBelts == null)
        {
            this.InputBelts = new Dictionary<HexSide, Belt>();
        }

        HashSet<HexSide> foundInputs = new HashSet<HexSide>();
        for (int i = 0; i < 6; i++)
        {
            Vector2Int neighbor = Helpers.GetNeighborPosition(this.Owner.GridPosition.x, this.Owner.GridPosition.y, (HexSide)i);
            Building neighborBuilding = Managers.Board.GetBuilding(neighbor);
            if (neighborBuilding?.ConveyorCell != null && neighborBuilding.ConveyorCell.Next == this)
            {
                HexSide neighborOutputSide = CalculateHexSide(neighborBuilding.transform.position, this.Owner.transform.position);
                HexSide inputSide = GetOppositeSide(neighborOutputSide);
                if (!this.InputBelts.ContainsKey(inputSide))
                {
                    this.InputBelts[inputSide] = new Belt(
                        new LinkedList<ItemOnBelt>(),
                        GetInputPath(inputSide, this.Owner.transform.position),
                        inputSide);
                }
                foundInputs.Add(inputSide);
            }
        }

        for (int i = 0; i < 6; i++)
        {
            if (!foundInputs.Contains((HexSide)i) && this.InputBelts.ContainsKey((HexSide)i))
            {
                this.InputBelts.Remove((HexSide)i);
            }
        }
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

    private static List<Vector3> GetInputPath(HexSide side, Vector3 center)
    {
        List<Vector3> points = GetPointsForSide(side);
        points.Reverse();
        for (int i = 0; i < points.Count; i++)
        {
            points[i] = points[i] + center;
        }

        return points;
    }

    private static List<Vector3> GetOutputPath(HexSide side, Vector3 center)
    {
        List<Vector3> points = GetPointsForSide(side);
        for (int i = 0; i < points.Count; i++)
        {
            points[i] = points[i] + center;
        }

        return points;
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