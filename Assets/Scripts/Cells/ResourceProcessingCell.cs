using UnityEngine;

public abstract class ResourceProcessingCell : Cell
{
    public abstract InventoryCell InputInventory { get; }
    public abstract ResourceType OutputResourceType { get; }
    public abstract ResourceType InputResourceType { get; }
    public abstract float SecondsToProcessResource { get; }
    private float processingStartTime;
    private bool isProcessingItem;

    public override void Update()
    {
        var furthestResource = this.Owner.ConveyorCell.GetFurthestAlongResourceOfType(InputResourceType);
        if (furthestResource != null && furthestResource.ProgressPercent > .2f)
        {
            Resource firstProcessable = InputInventory.GetFirstItem(InputResourceType);

            if (firstProcessable != null && !isProcessingItem)
            {
                processingStartTime = Time.time;
                furthestResource.IsPaused = true;
                isProcessingItem = true;
            }

            if (isProcessingItem && Time.time > processingStartTime + SecondsToProcessResource)
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
                furthestResource.IsPaused = false;

                isProcessingItem = false;
            }
        }
        else if (isProcessingItem)
        {
            // we should alays enter the first condition if we're processing an item.
            // If not, the items is gone.
            isProcessingItem = false;
        }
    }
}