public class ArrowShaft : Item
{
    public override ItemType Type => ItemType.ArrowShaft;
    public override float Width => .08f;
    public override ItemType[] Ingredients => _ingredients;
    private static ItemType[] _ingredients = new ItemType[] {ItemType.Log};
}