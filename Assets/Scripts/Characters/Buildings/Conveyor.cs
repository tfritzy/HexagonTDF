using System.Collections.Generic;
using UnityEngine;

public class Conveyor : Building
{
    public override BuildingType Type => BuildingType.Conveyor;
    public override BrainCell BrainCell => null;
    public override AttackCell AttackCell => null;
    public override LifeCell LifeCell => lifeCell;
    public override ResourceCollectionCell ResourceCollectionCell => null;
    public override ConveyorCell ConveyorCell => conveyorCell;
    public override Alliance Enemies => Alliance.Maltov;
    public override Alliance Alliance => Alliance.Player;

    private LifeCell lifeCell;
    private ConveyorCell conveyorCell;
    protected override void Setup()
    {
        lifeCell = new ConveyorLifeCell();
        conveyorCell = new ConveyorCell(false);

        base.Setup();
    }
}