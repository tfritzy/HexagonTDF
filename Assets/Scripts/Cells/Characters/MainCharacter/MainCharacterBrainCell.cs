using System.Collections.Generic;
using UnityEngine;

public class MainCharacterBrainCell : BrainCell
{
    private LinkedList<CharacterAction> CurrentActions = new LinkedList<CharacterAction>();

    public MainCharacterBrainCell()
    {

    }

    public void SetTargetHex(Vector2Int pos)
    {
        HexagonMono hex = Managers.Board.GetHex(pos);
        if (this.Owner.ResourceCollectionCell != null &&
            this.Owner.ResourceCollectionCell.BaseCollectionDetails.ContainsKey(hex.Biome))
        {
            ((MainCharacterResourceCollectionCell)this.Owner.ResourceCollectionCell).ChangeTargetHex(pos);

            this.CurrentActions = new LinkedList<CharacterAction>();
            this.CurrentActions.AddLast(new MoveAction(this.Owner, pos, stopOneBefore: true));
            this.CurrentActions.AddLast(new HarvestAction(this.Owner));
        }
        else
        {
            this.CurrentActions.AddLast(new MoveAction(this.Owner, pos, stopOneBefore: false));
        }
    }

    public override void Update()
    {
        if (this.CurrentActions.Count > 0)
        {
            if (this.CurrentActions.First.Value.State == CharacterAction.ActionState.Finished)
            {
                this.CurrentActions.RemoveFirst();
                return;
            }

            this.CurrentActions.First.Value.Update();
        }
    }
}