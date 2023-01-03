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

        HexagonMono hex = Managers.Board.GetHex(pos);
        if (this.Owner.ResourceCollectionCell != null &&
            this.Owner.ResourceCollectionCell.CanHarvestFrom(hex.Hexagon))
        {
            harvestCell.ChangeTargetHex(pos, hex.Biome);

            this.CurrentActions = new LinkedList<CharacterAction>();
            this.CurrentActions.AddLast(new MoveAction(this.Owner, pos, stopOneBefore: true));
            this.CurrentActions.AddLast(new HarvestAction(this.Owner));
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
                return;
            }

            this.CurrentActions.First.Value.Update();
        }
    }
}