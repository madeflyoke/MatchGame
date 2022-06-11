public interface IPausable
{
    public bool IsPaused { get; set; }
    public abstract void Pause(bool isPaused);
}
