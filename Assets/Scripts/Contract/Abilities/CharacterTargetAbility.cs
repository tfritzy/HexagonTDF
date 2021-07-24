using UnityEngine;

public abstract class CharacterTargetAbility : TargetAbility
{
    protected override string WaitingForInputMessage => WAITING_FOR_TARGET_MESSAGE;
    private const string WAITING_FOR_TARGET_MESSAGE = "Select a target.";
    public CharacterTargetAbility(Hero owner) : base(owner) { }

    protected override bool IsValidTarget(GameObject gameObject)
    {
        if (gameObject.TryGetComponent<Character>(out Character character))
        {
            return character.Alliance == this.Owner.Enemies;
        }

        return false;
    }
}