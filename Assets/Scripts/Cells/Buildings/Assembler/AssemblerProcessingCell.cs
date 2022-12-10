public class AssemblerProcessingCell : ResourceProcessingCell
{
    public override ItemType OutputItemType => ItemType.Arrow;
    public override float SecondsToProcessResource => .5f;
    public override float PercentOfInputConsumedPerOutput => 1f;
}