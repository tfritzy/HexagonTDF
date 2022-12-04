public class ArrowHead : Item
{
    public override ItemType Type => ItemType.ArrowHead;
    public override float Width => .1f;
    public override ItemType[] Ingredients => _ingredients;
    private static ItemType[] _ingredients = new ItemType[] {ItemType.Rock};
}