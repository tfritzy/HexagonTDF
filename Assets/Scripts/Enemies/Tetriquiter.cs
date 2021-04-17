using System.Collections.Generic;

public class Tetriquiter : Enemy
{
    public override EnemyType Type => EnemyType.Tetriquiter;
    public override VerticalRegion Region => VerticalRegion.Ground;

    public override Dictionary<AttributeType, float> PowerToAttributeRatio => powerToAttributeRatio;
    private static Dictionary<AttributeType, float> powerToAttributeRatio = new Dictionary<AttributeType, float>()
    {
        {AttributeType.Health, 1f},
    };
}