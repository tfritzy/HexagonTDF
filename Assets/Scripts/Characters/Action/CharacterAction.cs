public abstract class CharacterAction
{
    public Character Owner;
    public ActionState State;
    public enum ActionState
    {
        Unstarted,
        InProgress,
        Finished,
    }
    public abstract MainCharacterAnimationState Animation { get; }

    public CharacterAction(Character owner)
    {
        this.Owner = owner;
        this.State = ActionState.Unstarted;
    }

    public virtual void Start()
    {
        this.State = ActionState.InProgress;
        this.Owner.Animator?.SetInteger(Constants.AnimationStateParameter, (int)this.Animation);
    }

    public virtual void Update() { }

    public virtual void End()
    {
        this.State = ActionState.Finished;
    }
}