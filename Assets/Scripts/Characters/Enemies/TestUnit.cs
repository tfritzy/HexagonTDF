public class TestUnit : Unit
{
    public override BrainCell BrainCell => _brainCell;
    public override LifeCell LifeCell => _lifeCell;
    public override Alliance Enemies => Alliance.Player;
    public override Alliance Alliance => Alliance.Maltov;
    public override string Name => _name;
    private const string _name = "Test Unit";

    private TestUnitLifeCell _lifeCell;
    private UnitBrainCell _brainCell;

    public override void Setup()
    {
        this._lifeCell = new TestUnitLifeCell();
        this._brainCell = new UnitBrainCell();

        base.Setup();
    }
}