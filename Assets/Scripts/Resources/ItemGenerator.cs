public static class ItemGenerator {
    public static Item GetItemScript(ItemType type)
    {
        switch (type)
        {
            case (ItemType.Log):
                return new Log();
            case (ItemType.Sticks):
                return new Stick();
            default:
                throw new System.Exception("Unknown item type: " + type);
        }
    }
}