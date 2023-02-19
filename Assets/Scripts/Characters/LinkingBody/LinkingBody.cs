using UnityEngine;
using System.Collections.Generic;

public abstract class LinkingBody : MonoBehaviour
{
    public Character Owner;
    public HashSet<HexSide> NeighboredSides;

    public virtual void Setup()
    {
        CalculateCase();
        InformNeighborsOfChange();
    }

    protected abstract void SetupBody();

    public void CalculateCase()
    {
        if (Owner == null)
        {
            return;
        }

        NeighboredSides = new HashSet<HexSide>();
        for (int i = 0; i < 6; i++)
        {
            Vector2Int neighborPos = Helpers.GetNeighborPosition(Owner.GridPosition, (HexSide)i);
            Building neighbor = Managers.Board.GetBuilding(neighborPos);
            if (neighbor == null)
            {
                continue;
            }

            if (neighbor.Body.TryGetComponent<LinkingBody>(out LinkingBody linkingBody))
            {
                if (linkingBody.GetType() == this.GetType())
                {
                    NeighboredSides.Add((HexSide)i);
                }
            }
        }

        SetupBody();
    }

    private void InformNeighborsOfChange()
    {
        if (Owner == null)
        {
            return;
        }

        for (int i = 0; i < 6; i++)
        {
            Vector2Int neighborPos = Helpers.GetNeighborPosition(Owner.GridPosition, (HexSide)i);
            Building neighbor = Managers.Board.GetBuilding(neighborPos);
            if (neighbor == null)
            {
                continue;
            }

            if (neighbor.TryGetComponent<LinkingBody>(out LinkingBody linkingBody))
            {
                linkingBody.CalculateCase();
            }
        }
    }
}