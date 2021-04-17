using System.Collections.Generic;

public class Dode : Enemy
{
    public override EnemyType Type => EnemyType.Dode;
    public override VerticalRegion Region => VerticalRegion.Ground;
    public override Dictionary<AttributeType, float> PowerToAttributeRatio => powerToAttributeRatio;
    private static Dictionary<AttributeType, float> powerToAttributeRatio = new Dictionary<AttributeType, float>()
    {
        {AttributeType.Health, .8f},
        {AttributeType.MovementSpeed, .2f},
    };
}