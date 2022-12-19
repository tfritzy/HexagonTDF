using System.Collections.Generic;
using UnityEngine;

public class Conveyor : Building
{
    public override BuildingType Type => BuildingType.Conveyor;
    public override LifeCell LifeCell => lifeCell;
    public override ConveyorCell ConveyorCell => conveyorCell;
    public override bool RequiresConfirmationToBuild => false;
    public override Alliance Enemies => Alliance.Maltov;
    public override Alliance Alliance => Alliance.Player;
    public override string Name => charName;
    private const string charName = "Conveyor";

    private LifeCell lifeCell;
    private ConveyorCell conveyorCell;
    public override void Setup()
    {
        lifeCell = new ConveyorLifeCell();
        conveyorCell = new ConveyorCell(false);

        base.Setup();
    }
}