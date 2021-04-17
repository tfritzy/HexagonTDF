using System.Collections.Generic;

public class Icid : Enemy
{
    public override EnemyType Type => EnemyType.Icid;
    public override VerticalRegion Region => VerticalRegion.Air;
    public override Dictionary<AttributeType, float> PowerToAttributeRatio => powerToAttributeRatio;
    private static Dictionary<AttributeType, float> powerToAttributeRatio = new Dictionary<AttributeType, float>()
    {
        {AttributeType.Health, .66f},
        {AttributeType.Flies, .33f},
    };
}