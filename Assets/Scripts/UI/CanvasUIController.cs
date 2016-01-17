using UnityEngine;

public class CanvasUIController : MonoBehaviour
{
    #region Inspector
    
    
    [SerializeField]
    private ProgressUIController progress;
    
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
        Debug.Assert(this.progress);

        Application.targetFrameRate = this.frameRate;
    }
    
    
    #endregion


    #region Public


    public void Show(bool show)
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            var t = this.transform.GetChild(i);
            t.gameObject.SetActive(show);
        }
    }

    public void HandleLevelWin(bool newTimeRecord)
    {
        this.source.clip = newTimeRecord ? this.newBestClip : this.winClip;
        AudioManager.Instance.Play(this.source);
    }
    
    public void ForceUpdateAll(LevelState level, int junkCount)
    {
        this.progress.ForceUpdateAll(level, junkCount);
    }

    public void PlayClickSound()
    {
        this.source.clip = this.clickClip;
        AudioManager.Instance.Play(this.source);
    }
    
    
    #endregion
    
    
    #region Events
    

    public void OnLevelStart(LevelState level, int junkCount)
    {
        this.Show(false);

        this.ForceUpdateAll(level, junkCount);

        this.source.clip = this.launchClip;
        AudioManager.Instance.Play(this.source);
    }
    
    public void OnLevelLoad(LevelState level, int junkCount)
    {
        this.ForceUpdateAll(level, junkCount);
    }


    #endregion
}
