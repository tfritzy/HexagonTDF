using UnityEngine;

public class MainCharacter : Character
{
    public override LifeCell LifeCell => _lifeCell;
    public override BrainCell BrainCell => _brainCell;
    public override Alliance Enemies => Alliance.Maltov;
    public override Alliance Alliance => Alliance.Player;
    public override string Name => "You";

    private LifeCell _lifeCell;
    private MainCharacterBrainCell _brainCell;

    public override void Setup()
    {
        _lifeCell = new MainCharacterLifeCell();
        _brainCell = new MainCharacterBrainCell();

        base.Setup();
    }

    public override void SelectedClickHex(Vector2Int pos)
    {
        this._brainCell.SetPath(Managers.Board.ShortestPathBetween(this.GridPosition, pos));
    }
}