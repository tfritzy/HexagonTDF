public abstract class Cell
{
    public bool IsEnabled { get; private set; }
    public Character Owner { get; private set; }

    public virtual void Setup(Character owner)
    {
        this.Owner = owner;
        this.IsEnabled = true;
    }

    public abstract void Update();

    public void SetEnabled(bool enabled)
    {
        this.IsEnabled = enabled;
    }
}