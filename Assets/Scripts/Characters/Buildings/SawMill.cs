using System.Collections.Generic;

public class SawMill : Building
{
    public override LifeCell LifeCell => lifeCell;
    public override Alliance Enemies => Alliance.Maltov;
    public override Alliance Alliance => Alliance.Player;
    public override BuildingType Type => BuildingType.SawMill;
    public override ResourceProcessingCell ResourceProcessingCell => resourceProcessingCell;
    public override ConveyorCell ConveyorCell => conveyorCell;
    public override string Name => charName;
    public override Dictionary<ItemType, int> ItemsNeededForConstruction => _itemsNeededForConstruction;
    private Dictionary<ItemType, int> _itemsNeededForConstruction = new Dictionary<ItemType, int>
    {
        {ItemType.Log, 10},
        {ItemType.Plank, 20},
        {ItemType.StoneBlock, 30},
        {ItemType.SawBlade, 1},
        {ItemType.Shingle, 50},
    };

    private const string charName = "Saw mill";

    private LifeCell lifeCell;
    private ConveyorCell conveyorCell;
    private ResourceProcessingCell resourceProcessingCell;
    public override void Setup()
    {
        lifeCell = new SawMillLifeCell();
        resourceProcessingCell = new SawMillProcessingCell();
        conveyorCell = new ConveyorCell();

        base.Setup();
    }
}