using UnityEngine;

public abstract class HexagonTargetAbility : TargetAbility
{
    protected override string WaitingForInputMessage => INPUT_MESSAGE;
    private const string INPUT_MESSAGE = "Select a hex.";

    public HexagonTargetAbility(Hero hero) : base(hero) { }

    protected override bool IsValidTarget(GameObject gameObject)
    {
        return gameObject.GetComponent<HexagonMono>() != null;
    }
}