using UnityEngine;

public class Arrow : Item
{
    public override ItemType Type => ItemType.Arrow;
    public override ItemType[] Ingredients => _ingredients;
    private static ItemType[] _ingredients = new ItemType[] { ItemType.ArrowHead, ItemType.ArrowShaft };
    public override float Width => .05f;
    public override int MaxStackSize => 16;
    public override Quaternion ForwardOnConveyor => _forward;
    private static Quaternion _forward = Quaternion.Euler(0, 90, 0);
}