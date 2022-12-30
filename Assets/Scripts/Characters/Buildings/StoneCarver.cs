using System.Collections.Generic;

public class StoneCarver : Building
{
    public override LifeCell LifeCell => lifeCell;
    public override Alliance Enemies => Alliance.Maltov;
    public override Alliance Alliance => Alliance.Player;
    public override BuildingType Type => BuildingType.StoneCarver;
    public override ConveyorCell ConveyorCell => conveyorCell;
    public override ResourceProcessingCell ResourceProcessingCell => resourceProcessingCell;
    public override string Name => charName;
    private const string charName = "Stone carver";
    public override Dictionary<ItemType, int> ItemsNeededForConstruction => _itemsNeededForConstruction;
    private Dictionary<ItemType, int> _itemsNeededForConstruction = new Dictionary<ItemType, int>
    {
        {ItemType.Log, 10},
        {ItemType.Plank, 20},
        {ItemType.StoneBlock, 30},
        {ItemType.SawBlade, 1},
        {ItemType.Shingle, 50},
    };

    private LifeCell lifeCell;
    private ConveyorCell conveyorCell;
    private ResourceProcessingCell resourceProcessingCell;
    public override void Setup()
    {
        lifeCell = new StoneCarverLifeCell();
        resourceProcessingCell = new StoneCarverResourceProcessingCell();
        conveyorCell = new ConveyorCell(false);

        base.Setup();
    }
}