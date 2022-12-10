using System;

public abstract class Item
{
    public Guid Id;
    public abstract ItemType Type {get;}
    public virtual float Width => .2f;
    public virtual ItemType[] Ingredients => _emptyIngredients;
    public float RemainingPercent;
    private ItemType[] _emptyIngredients = new ItemType[0];
    public virtual int MaxStackSize => 1;
    public int Quantity;

    public Item()
    {
        this.Id = Guid.NewGuid();
        this.RemainingPercent = 1f;
        this.Quantity = 1;
    }
}