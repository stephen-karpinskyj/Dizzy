public abstract class LevelState
{
    protected string LevelId { get; private set; }
    
    protected LevelState(string levelId)
    {
        this.LevelId = levelId;
    }
    
    public abstract void ResetProgress();
}
