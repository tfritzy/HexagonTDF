using UnityEngine;

public class MainCharacter : Character
{
    public override LifeCell LifeCell => _lifeCell;
    public override BrainCell BrainCell => _brainCell;
    public override ResourceCollectionCell ResourceCollectionCell => _resourceCollectionCell;
    public override MovementCell MovementCell => _movementCell;
    public override InventoryCell InventoryCell => _inventoryCell;
    public override Alliance Enemies => Alliance.Maltov;
    public override Alliance Alliance => Alliance.Player;
    public override string Name => "You";

    private LifeCell _lifeCell;
    private MainCharacterBrainCell _brainCell;
    private MainCharacterResourceCollectionCell _resourceCollectionCell;
    private MovementCell _movementCell;
    private InventoryCell _inventoryCell;

    public override void Setup()
    {
        _lifeCell = new MainCharacterLifeCell();
        _brainCell = new MainCharacterBrainCell();
        _movementCell = new MainCharacterMovementCell();
        _inventoryCell = new InventoryCell(16);

        _resourceCollectionCell = new MainCharacterResourceCollectionCell();
        _resourceCollectionCell.SetEnabled(false); // Only becomes enabled when character is harvesting.

        base.Setup();
    }

    public override void SelectedClickHex(Vector2Int pos)
    {
        this._brainCell.SetTargetHex(pos);
    }
}