public static class ItemGenerator {
    public static Item Make(ItemType type)
    {
        switch (type)
        {
            case (ItemType.Log):
                return new Log();
            case (ItemType.Sticks):
                return new Stick();
            case (ItemType.Rock):
                return new Rock();
            default:
                throw new System.Exception("Unknown item type: " + type);
        }
    }
}