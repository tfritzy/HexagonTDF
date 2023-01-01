using System.Collections.Generic;
using UnityEngine;

public abstract class ResourceCollectionCell : Cell
{
    public abstract List<Vector2Int> HexesCollectedFrom { get; }
    public Dictionary<Biome, CollectionDetails> CurrentCollectionDetails { get; private set; }
    public virtual InventoryCell OutputInventory => this.Owner.InventoryCell;
    public abstract Dictionary<Biome, CollectionDetails> BaseCollectionDetails { get; }

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

        CurrentCollectionDetails = new Dictionary<Biome, CollectionDetails>();
        foreach (Vector2Int pos in this.HexesCollectedFrom)
        {
            var hex = Managers.Board.GetHex(pos);

            if (hex != null && BaseCollectionDetails.ContainsKey(hex.Biome))
            {
                ItemType collectedItem = BaseCollectionDetails[hex.Biome].Item;

                if (!CurrentCollectionDetails.ContainsKey(hex.Biome))
                {
                    CurrentCollectionDetails[hex.Biome] = new CollectionDetails
                    {
                        Item = BaseCollectionDetails[hex.Biome].Item,
                        TimeRequired = BaseCollectionDetails[hex.Biome].TimeRequired,
                    };
                }
                else
                {
                    CurrentCollectionDetails[hex.Biome].TimeRequired *= .75f;
                }
            }
        }
    }

    private float lastHarvestCheckTime;
    private Dictionary<Biome, float> lastCollectionTimes = new Dictionary<Biome, float>();
    private void HarvestResources()
    {
        if (Time.time < lastHarvestCheckTime + .25f)
        {
            return;
        }
        lastHarvestCheckTime = Time.time;

        if (CurrentCollectionDetails == null)
        {
            return;
        }

        foreach (Biome biome in this.CurrentCollectionDetails.Keys)
        {
            if (!lastCollectionTimes.ContainsKey(biome))
            {
                lastCollectionTimes[biome] = 0f;
            }

            if (Time.time - lastCollectionTimes[biome] > CurrentCollectionDetails[biome].TimeRequired &&
                this.OutputInventory.CanAcceptItem(CurrentCollectionDetails[biome].Item))
            {
                Item item = ItemGenerator.Make(CurrentCollectionDetails[biome].Item);
                this.OutputInventory.AddItem(item);
                lastCollectionTimes[biome] = Time.time;
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