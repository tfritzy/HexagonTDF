using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Source : Building
{
    public override BuildingType Type => BuildingType.Source;
    public override Alliances Alliance => Alliances.Player;
    public override Alliances Enemies => Alliances.Illigons;
}
