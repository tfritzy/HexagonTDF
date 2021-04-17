using System.Collections.Generic;

public class Octahedor : Enemy
{
    public override EnemyType Type => EnemyType.Octahedor;
    public override VerticalRegion Region => VerticalRegion.Ground;
    public override Dictionary<AttributeType, float> PowerToAttributeRatio => powerToAttributeRatio;
    private static Dictionary<AttributeType, float> powerToAttributeRatio = new Dictionary<AttributeType, float>()
    {
        {AttributeType.Health, 2f},
        {AttributeType.MovementSpeed, -.5f},
    };
}