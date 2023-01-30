using System.Collections.Generic;
using UnityEngine;

public class MainCharacterBrainCell : BrainCell
{
    private LinkedList<CharacterAction> CurrentActions = new LinkedList<CharacterAction>();
    private MainCharacterResourceCollectionCell harvestCell => (MainCharacterResourceCollectionCell)this.Owner.ResourceCollectionCell;

    public MainCharacterBrainCell()
    {

    }

    public void SetTargetHex(Vector2Int pos)
    {
        foreach (CharacterAction action in this.CurrentActions)
        {
            action.End();
        }

        CurrentActions = new LinkedList<CharacterAction>();

        Managers.Board.World.TryGetHexBody(pos.x, pos.y, out HexagonMono hex);
        if (this.Owner.ResourceCollectionCell != null &&
            this.Owner.ResourceCollectionCell.CanHarvestFrom(hex.Hexagon))
        {
            harvestCell.ChangeTargetHex(pos, hex.Biome);

            this.CurrentActions = new LinkedList<CharacterAction>();
            this.CurrentActions.AddLast(new MoveAction(this.Owner, pos, stopOneBefore: true));
            this.CurrentActions.AddLast(new HarvestAction(this.Owner, hex.Biome));
        }
        else if (Managers.Board.GetBuilding(hex.GridPosition) != null)
        {
            this.CurrentActions.AddLast(new MoveAction(this.Owner, pos, stopOneBefore: true));
            this.CurrentActions.AddLast(new OpenInventoryAction(this.Owner, Managers.Board.GetBuilding(hex.GridPosition)));
        }
        else
        {
            this.CurrentActions.AddLast(new MoveAction(this.Owner, pos, stopOneBefore: false));
        }

        this.CurrentActions.First.Value.Start();
    }

    public override void Update()
    {
        if (this.CurrentActions.Count > 0)
        {
            if (this.CurrentActions.First.Value.State == CharacterAction.ActionState.Finished)
            {
                this.CurrentActions.RemoveFirst();
                this.CurrentActions.First?.Value.Start();

                if (this.CurrentActions.Count == 0)
                {
                    this.Owner.Animator?.SetInteger(Constants.AnimationStateParameter, (int)MainCharacterAnimationState.Idle);
                }

                return;
            }

            this.CurrentActions.First.Value.Update();
        }


    }
}