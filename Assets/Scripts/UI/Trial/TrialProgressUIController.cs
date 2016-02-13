using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TrialProgressUIController : MonoBehaviour
{
    #region Inspector
    
    
    [SerializeField]
    private GameObject parent;
    
    [SerializeField]
    private Text bestTimeText;

    [SerializeField]
    private Text bestTimeDiffText;

    [SerializeField]
    private Text lastTimeText;

    [SerializeField]
    private Text runCountText;
    
    [SerializeField]
    private TrialGoalUIController[] trialGoals;
    
    [SerializeField]
    private Color normalTimeColor;

    [SerializeField]
    private Color newTimeColor;

    [SerializeField]
    private float timeChangeHalfFlashDuration = 0.25f;

    [SerializeField]
    private float timeChangeFlashCount = 2;


    #endregion


    #region Unity


    private void Awake()
    {
        Debug.Assert(this.bestTimeText);
        Debug.Assert(this.lastTimeText);
        Debug.Assert(this.runCountText);
        Debug.Assert(this.trialGoals.Length > 0);
    }

    private void OnDisable()
    {
        this.StopAllCoroutines();
    }
    
    
    #endregion


    #region Public


    public void UpdateBestTime(float time, float diff)
    {
        this.StartCoroutine(this.FlashCoroutine(this.bestTimeText, FormattingUtility.TimeToString(time)));
        this.bestTimeDiffText.gameObject.SetActive(true);
        this.bestTimeDiffText.text = FormattingUtility.TimeDiffToString(diff);
    }

    public void UpdateLastTime(float time)
    {
        this.StartCoroutine(this.FlashCoroutine(this.lastTimeText, FormattingUtility.TimeToString(time)));
    }

    public void UpdateRunCount(int count)
    {
        this.runCountText.text = FormattingUtility.RunCountToString(count);
    }
    
    public void UpdateGoals(TrialLevelState state)
    {
        var i = 0;
        
        if (state != null)
        {
            foreach (var g in state.ActiveGoals)
            {
                if (i >= this.trialGoals.Length)
                {
                    break;
                }
                
                this.trialGoals[i].UpdateData(g);
                
                i++;
            }
        }

        while (i < this.trialGoals.Length)
        {
            this.trialGoals[i].Hide();
            i++;
        }
    }

    public void ForceUpdateAll(TrialLevelState state)
    {
        var best = state == null ? 0f : state.BestTime;
        var last = state == null ? 0f : state.LastTime;
        var runs = state == null ? 0 : state.RunCount;
        
        this.bestTimeText.text = FormattingUtility.TimeToString(best);
        this.bestTimeText.color = this.normalTimeColor;
        this.bestTimeDiffText.gameObject.SetActive(false);
        this.lastTimeText.text = FormattingUtility.TimeToString(last);
        this.lastTimeText.color = this.normalTimeColor;
        this.runCountText.text = FormattingUtility.RunCountToString(runs);
        
        this.UpdateGoals(state);
    }
    
    public void Show(bool show)
    {
        this.parent.SetActive(show);
    }
    
    
    #endregion


    #region Coroutines
    

    private IEnumerator FlashCoroutine(Text text, string value)
    {
        text.text = value;

        for (int i = 0; i < this.timeChangeFlashCount; i++)
        {
            text.color = this.newTimeColor;
            yield return new WaitForSeconds(this.timeChangeHalfFlashDuration);
            text.color = this.normalTimeColor;
            yield return new WaitForSeconds(this.timeChangeHalfFlashDuration);
        }
    }
    
    
    #endregion
}
