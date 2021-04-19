using System.Collections;
using System.Collections.Generic;

public class BaseBuilderMenu : BuildMenu
{
    public override List<BuildingType> BuildingTypes
    {
        get { return buildingTypes; }
    }

    private readonly List<BuildingType> buildingTypes = new List<BuildingType>()
    {
        BuildingType.Forager,
        BuildingType.Farm,
        BuildingType.Lumbermill,
        BuildingType.StoneMine
    };
}
