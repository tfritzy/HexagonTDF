using System.Collections.Generic;

public class HarvestAction : CharacterAction
{
    public override MainCharacterAnimationState Animation => currentAnimation;

    private MainCharacterAnimationState currentAnimation;
    private static Dictionary<Biome, MainCharacterAnimationState> harvestAnimations = new Dictionary<Biome, MainCharacterAnimationState>()
    {
        {Biome.Forrest, MainCharacterAnimationState.Chopping},
        {Biome.Mountain, MainCharacterAnimationState.Mining},
    };

    public HarvestAction(Character owner, Biome targetBiome) : base(owner)
    {
        this.currentAnimation = harvestAnimations[targetBiome];
    }

    public override void Start()
    {
        base.Start();

        this.Owner.ResourceCollectionCell.SetEnabled(true);
    }

    public override void End()
    {
        base.End();

        this.Owner.ResourceCollectionCell.Reset();
        this.Owner.ResourceCollectionCell.SetEnabled(false);
    }
}