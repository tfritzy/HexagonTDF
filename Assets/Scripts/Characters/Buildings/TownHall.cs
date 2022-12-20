using System.Collections.Generic;

public class TownHall : Building
{
    public override BuildingType Type => BuildingType.TownHall;
    public override LifeCell LifeCell => _lifeCell;
    public override Alliance Enemies => Alliance.Maltov;
    public override Alliance Alliance => Alliance.Player;
    public override string Name => "Town Hall";
    public override List<HexSide> ExtraSize => _extraSize;
    private List<HexSide> _extraSize = new List<HexSide> { HexSide.NorthWest, HexSide.SouthWest };

    private LifeCell _lifeCell;

    public override void Setup()
    {
        base.Setup();

        this._lifeCell = new TownHallLifeCell();
    }
}