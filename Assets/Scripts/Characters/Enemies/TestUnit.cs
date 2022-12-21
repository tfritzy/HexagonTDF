public class TestUnit : Character
{
    public override BrainCell BrainCell => _brainCell;
    public override LifeCell LifeCell => _lifeCell;
    public override Alliance Enemies => Alliance.Player;
    public override Alliance Alliance => Alliance.Maltov;
    public override string Name => name;
    private const string name = "Test Unit";

    private TestUnitLifeCell _lifeCell;
    private CharacterBrainCell _brainCell;

    public override void Setup()
    {
        this._lifeCell = new TestUnitLifeCell();
        this._brainCell = new CharacterBrainCell();

        base.Setup();
    }
}