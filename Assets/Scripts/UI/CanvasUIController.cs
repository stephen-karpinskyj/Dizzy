using UnityEngine;

public class CanvasUIController : MonoBehaviour
{
    #region Inspector
    
    
    [SerializeField]
    private HeaderUIController header;
    
    [SerializeField]
    private ProgressUIController progress;
    
    [SerializeField]
    private TimerUIController timer;
    
    [SerializeField]
    private AudioSource source;

    [SerializeField]
    private AudioClip winClip;

    [SerializeField]
    private AudioClip newBestClip;

    [SerializeField]
    private AudioClip clickClip;

    [SerializeField]
    private AudioClip launchClip;

    [SerializeField]
    private int frameRate = 60;
    
    
    #endregion
    
    
    #region Properties
    
    
    public ProgressUIController Progress
    {
        get { return this.progress; }
    }
    
    
    #endregion
    
    
    #region Unity


    private void Awake()
    {
        Debug.Assert(this.header);
        Debug.Assert(this.progress);
        Debug.Assert(this.timer);

        Application.targetFrameRate = this.frameRate;
    }
    
    
    #endregion


    #region Public


    public void HandleLevelWin(bool newTimeRecord)
    {
        this.source.clip = newTimeRecord ? this.newBestClip : this.winClip;
        AudioManager.Instance.Play(this.source);
    }
    
    public void ForceUpdateAll(LevelData data, LevelState state, int junkCount)
    {
        this.header.UpdateLevelTitleText(data.DisplayName);
        this.progress.ForceUpdateAll(state, junkCount);
    }

    public void PlayClickSound()
    {
        this.source.clip = this.clickClip;
        AudioManager.Instance.Play(this.source);
    }
    
    
    #endregion
    
    
    #region Private
    
    
    private void Show(bool show)
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            var t = this.transform.GetChild(i);
            t.gameObject.SetActive(show);
        }
    }
    
    
    #endregion
    
    
    #region Events
    

    public void OnLevelStart(LevelData data, LevelState state, int junkCount)
    {
        var isTrial = data is TrialLevelData;
        
        this.Show(false);

        if (isTrial)
        {
            this.timer.StartTimer(state.BestTime);
        }
        
        this.ForceUpdateAll(data, state, junkCount);

        this.source.clip = this.launchClip;
        AudioManager.Instance.Play(this.source);
    }
    
    public void OnLevelStop(LevelData data)
    {
        this.Show(true);
        
        this.timer.StopTimer();
        
        this.progress.OnLevelStop(data);
    }
    
    public void OnLevelLoad(LevelData data, LevelState state, int junkCount)
    {
        this.progress.OnLevelLoad(data);
        this.ForceUpdateAll(data, state, junkCount);
    }


    #endregion
}
