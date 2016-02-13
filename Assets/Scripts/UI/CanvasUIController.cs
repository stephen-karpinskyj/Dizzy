using UnityEngine;

public class CanvasUIController : MonoBehaviour
{
    #region Inspector
    
    
    [SerializeField]
    private HeaderUIController header;
    
    [SerializeField]
    private GlobalJunkUIController junk;
    
    [SerializeField]
    private TrialProgressUIController trialProgress;
    
    [SerializeField]
    private ExplorationProgressUIController explorationProgress;
    
    [SerializeField]
    private TimerUIController timer;
    
    [SerializeField]
    private BeaconPingUIController beaconPing;
    
    [SerializeField]
    private Transform[] childrenToHideInGame;
    
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
    
    
    #endregion
    
    
    #region Properties
    
    
    public GlobalJunkUIController Junk
    {
        get { return this.junk; }
    }
    
    public TrialProgressUIController TrialProgress
    {
        get { return this.trialProgress; }
    }
    
    public ExplorationProgressUIController ExplorationProgress
    {
        get { return this.explorationProgress; }
    }
    
    
    #endregion
    
    
    #region Unity


    private void Awake()
    {
        Debug.Assert(this.header);
        Debug.Assert(this.junk);
        Debug.Assert(this.trialProgress);
        Debug.Assert(this.explorationProgress);
        Debug.Assert(this.timer);
        Debug.Assert(this.beaconPing);
    }
    
    
    #endregion


    #region Public


    public void HandleTrialWin(bool newTimeRecord)
    {
        this.source.clip = newTimeRecord ? this.newBestClip : this.winClip;
        AudioManager.Instance.Play(this.source);
    }
    
    public void ForceUpdateAll(LevelData data, LevelState state, ulong junkCount, float junkMultiplier)
    {
        this.header.UpdateLevelTitleText(data.DisplayName);
        this.junk.ForceUpdateAll(junkCount, junkMultiplier);
        
        var trialState = state as TrialLevelState;
        
        if (trialState != null)
        {
            this.trialProgress.ForceUpdateAll(trialState);
        }
        else
        {
            this.explorationProgress.ForceUpdateAll(data as ExplorationLevelData, state as ExplorationLevelState, junkCount);
        }
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
        foreach (var t in this.childrenToHideInGame)
        {
            t.gameObject.SetActive(show);
        }
    }
    
    
    #endregion
    
    
    #region Events
    

    public void OnLevelStart(LevelData data, LevelState state, ulong junkCount, float junkMultiplier)
    {
        var trialState = state as TrialLevelState;
        
        if (trialState != null)
        {
            this.timer.StartTimer(trialState.BestTime);
        }
        else
        {
            var explorationState = state as ExplorationLevelState;
            
            if (explorationState.IsBeaconPurchased)
            {
                this.beaconPing.Show(true);
            }
        }
        
        this.ForceUpdateAll(data, state, junkCount, junkMultiplier);

        this.source.clip = this.launchClip;
        AudioManager.Instance.Play(this.source);
        
        this.Show(false);
    }
    
    public void OnLevelStop(LevelData data)
    {
        this.Show(true);
        
        this.timer.StopTimer();
        this.beaconPing.Show(false);
    }
    
    public void OnLevelLoad(LevelData data, LevelState state, ulong junkCount, float junkMultiplier)
    {
        this.ForceUpdateAll(data, state, junkCount, junkMultiplier);
        
        var explorationData = data as ExplorationLevelData;
        var isTrial = explorationData == null;
        
        this.TrialProgress.Show(isTrial);
        this.ExplorationProgress.Show(!isTrial);
        
        if (!isTrial)
        {
            this.beaconPing.Initialise(explorationData.Beacon.Target);
        }
    }


    #endregion
}
