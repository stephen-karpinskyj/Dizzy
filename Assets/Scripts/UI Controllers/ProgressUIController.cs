using UnityEngine;
using UnityEngine.UI;

public class ProgressUIController : MonoBehaviour
{
    [SerializeField]
    private Text bestTimeText;

    [SerializeField]
    private Text lastTimeText;

    [SerializeField]
    private Text runCountText;

    [SerializeField]
    private Text junkCountText;

    [SerializeField]
    private Text addedJunkCountText;
    
    [SerializeField]
    private MedalUIController noviceMedal;

    [SerializeField]
    private MedalUIController proMedal;

    private void Awake()
    {
        Debug.Assert(this.bestTimeText);
        Debug.Assert(this.lastTimeText);
        Debug.Assert(this.runCountText);
        Debug.Assert(this.junkCountText);
        Debug.Assert(this.addedJunkCountText);
        Debug.Assert(this.noviceMedal);
        Debug.Assert(this.proMedal);

        this.addedJunkCountText.gameObject.SetActive(false);
        
        this.HandleBestTimeChange(StateManager.Instance.BestTime);
        this.HandleLastTimeChange(StateManager.Instance.LastTime);
        this.HandleRunCountChange(StateManager.Instance.RunCount);
        this.HandleJunkCountChange(StateManager.Instance.JunkCount);
        this.HandleNoviceMedalEarntChange(StateManager.Instance.NoviceMedalEarnt);
        this.HandleProMedalEarntChange(StateManager.Instance.ProMedalEarnt);

        StateManager.Instance.OnBestTimeChange += this.HandleBestTimeChange;
        StateManager.Instance.OnLastTimeChange += this.HandleLastTimeChange;
        StateManager.Instance.OnRunCountChange += this.HandleRunCountChange;
        StateManager.Instance.OnJunkCountChange += this.HandleJunkCountChange;
        StateManager.Instance.OnNoviceMedalEarntChange += this.HandleNoviceMedalEarntChange;
        StateManager.Instance.OnProMedalEarntChange += this.HandleProMedalEarntChange;
    }

    private void HandleBestTimeChange(float time)
    {
        this.bestTimeText.text = StateManager.TimeToString(time);
    }

    private void HandleLastTimeChange(float time)
    {
        this.lastTimeText.text = StateManager.TimeToString(time);
    }

    private void HandleRunCountChange(int count)
    {
        this.runCountText.text = StateManager.RunCountToString(count);
    }

    private void HandleJunkCountChange(int count)
    {
        this.junkCountText.text = StateManager.JunkCountToString(count);
    }

    private void HandleNoviceMedalEarntChange(bool earnt)
    {
        this.noviceMedal.SetEarnt(earnt);
    }

    private void HandleProMedalEarntChange(bool earnt)
    {
        this.proMedal.SetEarnt(earnt);
    }
}
