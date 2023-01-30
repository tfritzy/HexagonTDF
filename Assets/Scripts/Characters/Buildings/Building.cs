using System.Collections.Generic;
using UnityEngine;

public abstract class Building : Character
{
    public override InventoryCell InventoryCell => _inventory;
    public override MovementCell MovementCell => null;
    public abstract BuildingType Type { get; }
    public virtual bool RequiresConfirmationToBuild => true;
    public virtual List<HexSide> ExtraSize => new List<HexSide>();
    public virtual int InventorySize => 8;
    private InventoryCell _inventory;

    // Construction
    public abstract Dictionary<ItemType, int> ItemsNeededForConstruction { get; }
    public bool ShouldInstaBuild;
    public virtual bool NeedsConstruction => true;
    public float ConstructionPercent { get; private set; }
    public bool IsConstructed => !NeedsConstruction || (numItemsUsedForConstruction >= numItemsNeededForConstruction);
    private Dictionary<ItemType, int> ItemsUsedForConstruction;
    private int numItemsNeededForConstruction;
    private int numItemsUsedForConstruction;
    private Material underConstructionMaterial;
    private Bounds _bodyBounds;
    private ConstructionProgress constructionProgressHoverer;
    private float renderedConstructionPercent;
    private float lastConstructionIncrementTime;
    private const string CONSTRUCTION_MAT_CONSTRUCTED_PROP = "_PercentConstructed";
    private const string CONSTRUCTION_MAT_MIN_Y = "_ModelMinY";

    public override void Setup()
    {
        _inventory = new InventoryCell(this.InventorySize, $"{this.Name} Inventory");
        base.Setup();
        this.gameObject.layer = Constants.Layers.BuildingsLayerIndex;
        InitConstruction();
    }

    protected override void UpdateLoop()
    {
        base.UpdateLoop();

        if (ShouldInstaBuild)
        {
            FinishConstruction();
            ShouldInstaBuild = false;
        }

        LerpConstructedPercent();
        CheckForConstructionResources();
    }

    private void InitConstruction()
    {
        if (!NeedsConstruction)
        {
            FinishConstruction();
            return;
        }

        this.ItemsUsedForConstruction = new Dictionary<ItemType, int>();
        this.SetMaterial(Prefabs.GetMaterial(MaterialType.UnderConstruction));
        this.underConstructionMaterial = this.Body.GetComponent<MeshRenderer>().material;
        this._bodyBounds = this.Body.GetComponent<MeshRenderer>().bounds;
        this.underConstructionMaterial.SetFloat(CONSTRUCTION_MAT_CONSTRUCTED_PROP, 0f);
        this.underConstructionMaterial.SetFloat(CONSTRUCTION_MAT_MIN_Y, this._bodyBounds.min.y);
        this.constructionProgressHoverer = (ConstructionProgress)Managers.UI.ShowHoverer(Hoverer.ConstructionProgress, this.transform);

        foreach (ItemType item in ItemsNeededForConstruction.Keys)
        {
            this.numItemsNeededForConstruction += ItemsNeededForConstruction[item];
        }
    }

    public Vector3 GetWorldPosition()
    {
        Vector3 center = Helpers.ToWorldPosition(this.GridPosition);
        foreach (HexSide side in ExtraSize)
        {
            center += Helpers.ToWorldPosition(Helpers.GetNeighborPosition(this.GridPosition, side));
        }

        center /= ExtraSize.Count + 1;
        Managers.Board.World.TryGetHex(this.GridPosition.x, this.GridPosition.y, out Hexagon hex);
        center.y = hex.Height * Constants.HEXAGON_HEIGHT;

        return center;
    }

    private float lastConstructionCheckTime;
    private const float constructionDebounceTime = .2f;
    private void CheckForConstructionResources()
    {
        if (IsConstructed)
        {
            return;
        }

        if (Time.time < lastConstructionCheckTime + constructionDebounceTime)
        {
            return;
        }
        lastConstructionCheckTime = Time.time;

        foreach (ItemType item in ItemsNeededForConstruction.Keys)
        {
            if (!ItemsUsedForConstruction.ContainsKey(item))
            {
                ItemsUsedForConstruction[item] = 0;
            }

            if (ItemsUsedForConstruction[item] < ItemsNeededForConstruction[item])
            {
                int firstIndex = this.InventoryCell.FirstIndexOfItem(item);
                if (firstIndex != -1)
                {
                    this.lastConstructionIncrementTime = Time.time;
                    this.numItemsUsedForConstruction += 1;
                    this.ItemsUsedForConstruction[item] += 1;
                    this.InventoryCell.RemoveAt(firstIndex);
                    this.ConstructionPercent = (float)numItemsUsedForConstruction / (float)numItemsNeededForConstruction;
                    this.constructionProgressHoverer.Update(ConstructionPercent * 100f);
                }
            }
        }

        if (IsConstructed)
        {
            FinishConstruction();
        }
    }

    public int GetRemainingItemsNeeded(ItemType itemType)
    {
        if (!ItemsNeededForConstruction.ContainsKey(itemType))
        {
            return 0;
        }
        else
        {
            if (ItemsUsedForConstruction.ContainsKey(itemType))
            {
                return ItemsNeededForConstruction[itemType] - ItemsUsedForConstruction[itemType];
            }
            else
            {
                return ItemsNeededForConstruction[itemType];
            }
        }
    }

    private void LerpConstructedPercent()
    {
        if (IsConstructed)
        {
            return;
        }

        renderedConstructionPercent = Mathf.Lerp(
            renderedConstructionPercent,
            ConstructionPercent,
            (Time.time - lastConstructionIncrementTime) * 4f);

        this.underConstructionMaterial.SetFloat(
            CONSTRUCTION_MAT_CONSTRUCTED_PROP,
            renderedConstructionPercent * _bodyBounds.extents.y * 2.2f);
    }

    private void FinishConstruction()
    {
        Managers.UI.HideHoverer(this.constructionProgressHoverer);
        if (numItemsUsedForConstruction >= numItemsNeededForConstruction)
        {
            this.Body.GetComponent<MeshRenderer>().material = Prefabs.GetMaterial(MaterialType.ColorPalette);
        }
    }
}