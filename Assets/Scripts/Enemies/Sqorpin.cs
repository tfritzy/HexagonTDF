using System.Collections.Generic;

public class Sqorpin : Enemy
{
    public override EnemyType Type => EnemyType.Sqorpin;
    public override VerticalRegion Region => VerticalRegion.Ground;
    public override Dictionary<AttributeType, float> PowerToAttributeRatio => powerToAttributeRatio;
    private static Dictionary<AttributeType, float> powerToAttributeRatio = new Dictionary<AttributeType, float>()
    {
        {AttributeType.Health, .5f},
        {AttributeType.MovementSpeed, .5f},
    };
}