using UnityEngine;

public abstract class ResourceProcessingCell : Cell
{
    public abstract ResourceType OutputResourceType { get; }
    public abstract ResourceType InputResourceType { get; }
    public abstract float SecondsToProcessResource { get; }
    private float processingStartTime;

    public override void Update()
    {
        var furthestResource = this.Owner.ConveyorCell.GetFurthestAlongResourceOfType(InputResourceType);
        if (furthestResource.ProgressPercent > .2f)
        {
            GameObject newResource = GameObject.Instantiate(
                Prefabs.GetResource(OutputResourceType),
                furthestResource.Resource.transform.position,
                Prefabs.GetResource(OutputResourceType).transform.rotation
            );
            GameObject.Destroy(furthestResource.Resource.gameObject);
            Resource resource = newResource.AddComponent<Resource>();
            resource.Init(OutputResourceType);
            furthestResource.Resource = resource;
        }
    }
}