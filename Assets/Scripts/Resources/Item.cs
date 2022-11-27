using System;

public abstract class Item
{
    public Guid Id;
    public abstract ItemType Type {get;}
    public virtual float Width => .2f;

    public Item()
    {
        this.Id = Guid.NewGuid();
    }
}