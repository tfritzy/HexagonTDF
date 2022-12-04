public class Arrow : Item
{
    public override ItemType Type => ItemType.Arrow;
    public override ItemType[] Ingredients => _ingredients;
    private static ItemType[] _ingredients = new ItemType[] {ItemType.ArrowShaft, ItemType.ArrowHead};
    public override float Width => .08f;
}