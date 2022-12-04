public static class ItemGenerator {
    public static Item Make(ItemType type)
    {
        switch (type)
        {
            case (ItemType.Log):
                return new Log();
            case (ItemType.ArrowShaft):
                return new ArrowShaft();
            case (ItemType.Rock):
                return new Rock();
            case (ItemType.ArrowHead):
                return new ArrowHead();
            case (ItemType.Arrow):
                return new Arrow();
            default:
                throw new System.Exception("Unknown item type: " + type);
        }
    }
}