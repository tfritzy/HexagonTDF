using UnityEngine;

public abstract class LinkingBody : MonoBehaviour
{
    protected Character Owner;
    public int LinkCase;

    void Start()
    {
        Setup();
    }

    protected virtual void Setup()
    {
        this.Owner = this.transform.parent?.GetComponent<Character>();
        CalculateCase();
        InformNeighborsOfChange();
    }

    public void CalculateCase()
    {
        if (Owner == null)
        {
            return;
        }

        LinkCase = 0;
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
                    LinkCase |= 1 << i;
                }
            }
        }
    }

    private void InformNeighborsOfChange()
    {
        if (Owner == null)
        {
            return;
        }

        LinkCase = 0;
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
                linkingBody.CalculateCase();
            }
        }
    }
}