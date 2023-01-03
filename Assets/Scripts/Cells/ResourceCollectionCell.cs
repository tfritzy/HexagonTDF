using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ResourceCollectionCell : Cell
{
    public abstract List<Vector2Int> HexesCollectedFrom { get; }
    public CollectionDetails CurrentCollectionDetails { get; private set; }
    public virtual InventoryCell OutputInventory => this.Owner.InventoryCell;
    public abstract bool CanHarvestFrom(Hexagon hexagon);
    public abstract CollectionDetails BaseCollectionDetails { get; }
    private HarvestProgress harvestHoverer;

    public class CollectionDetails
    {
        public ItemType Item;
        public float TimeRequired;
    }

    public override void Setup(Character character)
    {
        base.Setup(character);
        InitCollectionRates();
    }

    public override void Update()
    {
        HarvestResources();
    }

    protected void InitCollectionRates()
    {
        if (this.HexesCollectedFrom == null)
        {
            return;
        }

        this.CurrentCollectionDetails = null;
        foreach (Vector2Int pos in this.HexesCollectedFrom)
        {
            var hex = Managers.Board.GetHex(pos);

            if (hex != null && CanHarvestFrom(hex.Hexagon))
            {
                ItemType collectedItem = BaseCollectionDetails.Item;

                if (CurrentCollectionDetails == null)
                {
                    CurrentCollectionDetails = new CollectionDetails
                    {
                        Item = BaseCollectionDetails.Item,
                        TimeRequired = BaseCollectionDetails.TimeRequired,
                    };
                }
                else
                {
                    CurrentCollectionDetails.TimeRequired *= .75f;
                }
            }
        }
    }

    private float lastHarvestCheckTime;
    private float lastCollectionTime = 0f;
    private void HarvestResources()
    {
        if (Time.time < lastHarvestCheckTime + .25f &&
            !(Time.time - lastCollectionTime > CurrentCollectionDetails.TimeRequired))
        {
            return;
        }
        lastHarvestCheckTime = Time.time;

        if (CurrentCollectionDetails == null)
        {
            return;
        }

        if (lastCollectionTime == 0f)
        {
            // initial state
            lastCollectionTime = Time.time;
        }

        if (harvestHoverer == null)
        {
            harvestHoverer = (HarvestProgress)Managers.UI.ShowHoverer(Hoverer.HarvestProgress, this.Owner.transform);
        }

        harvestHoverer.Update(((Time.time - lastCollectionTime) / CurrentCollectionDetails.TimeRequired) * 100f);

        if (Time.time - lastCollectionTime > CurrentCollectionDetails.TimeRequired &&
            this.OutputInventory.CanAcceptItem(CurrentCollectionDetails.Item))
        {
            Item item = ItemGenerator.Make(CurrentCollectionDetails.Item);
            this.OutputInventory.AddItem(item);
            lastCollectionTime = Time.time;
        }

        if (this.Owner.ConveyorCell != null)
        {
            int firstItemIndex = this.OutputInventory.FirstNonEmptyIndex();
            if (firstItemIndex != -1 &&
                this.Owner.ConveyorCell.CanAccept(
                    this.Owner.ConveyorCell.OutputBelt,
                    this.OutputInventory.ItemAt(firstItemIndex).Width))
            {
                Item itemToPlace = this.OutputInventory.ItemAt(firstItemIndex);
                SpawnItem(itemToPlace);
                this.OutputInventory.RemoveAt(firstItemIndex);
            }
        }
    }

    public void Reset()
    {
        lastCollectionTime = 0f;
        Managers.UI.HideHoverer(this.harvestHoverer);
        this.harvestHoverer = null;
    }

    private void SpawnItem(Item item)
    {
        var resourceGO = GameObject.Instantiate(
            Prefabs.GetResource(item.Type),
            this.Owner.transform.position,
            new Quaternion());

        InstantiatedItem itemInst = resourceGO.AddComponent<InstantiatedItem>();
        itemInst.Init(item);
        this.Owner.ConveyorCell.AddItem(this.Owner.ConveyorCell.OutputBelt, itemInst);
    }
}