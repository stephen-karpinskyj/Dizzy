using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ProgressUIController : MonoBehaviour
{
    #region Inspector
    
    
    [SerializeField]
    private Text bestTimeText;

    [SerializeField]
    private Text bestTimeDiffText;

    [SerializeField]
    private Text lastTimeText;

    [SerializeField]
    private Text runCountText;

    [SerializeField]
    private Text junkCountText;

    [SerializeField]
    private Text addedJunkCountText;
    
    [SerializeField]
    private Text junkMultiplierText;
    
    [SerializeField]
    private Text addedJunkMultiplierText; 
    
    [SerializeField]
    private TrialLevelGoalUIController[] trialGoals;

    [SerializeField]
    private int junkAddRate = 300;
    
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
        Debug.Assert(this.junkCountText);
        Debug.Assert(this.addedJunkCountText);
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

    public void UpdateJunkCount(ulong count, ulong change)
    {
        this.StartCoroutine(this.JunkChangeCoroutine(count, change));
    }
    
    public void UpdateJunkMultiplier(float multiplier, float change = 0f)
    {
        this.junkMultiplierText.text = FormattingUtility.JunkMultiplierToString(multiplier);
        
        if (Mathf.Approximately(change, 0f))
        {
            this.addedJunkMultiplierText.text = string.Empty;
        }
        else
        {
            this.addedJunkMultiplierText.text = FormattingUtility.SignedJunkMultiplierToString(change);
        }
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

    public void ForceUpdateAll(TrialLevelState state, ulong junkCount, float junkMultiplier)
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

        this.junkCountText.text = FormattingUtility.JunkCountToString(junkCount);
        this.addedJunkCountText.gameObject.SetActive(false);
        
        this.UpdateJunkMultiplier(junkMultiplier);
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

    private IEnumerator JunkChangeCoroutine(ulong count, ulong change)
    {
        if (change == 0)
        {
            this.addedJunkCountText.gameObject.SetActive(false);
            yield break;
        }

        this.addedJunkCountText.gameObject.SetActive(true);
        this.addedJunkCountText.text = FormattingUtility.SignedJunkCountToString(change);

        var finalCount = count;
        count -= change;

        var dir = (int)Mathf.Sign(change);
        var changeMin = (dir == 1) ? 0 : change;
        var changeMax = (dir == 1) ? change : 0;
        var countMin = (dir == 1) ? count : finalCount;
        var countMax = (dir == 1) ? finalCount : count;

        var range = Math.Pow(this.junkAddRate, Math.Max(1, Math.Log10(count + change) - 0.5));

        while (count != finalCount)
        {
            var changeThisFrame = (ulong)(Time.deltaTime * range * dir);

            if (changeThisFrame == 0)
            {
                changeThisFrame = (ulong)(1 * dir);
            }

            change -= changeThisFrame;
            change = change < changeMin ? changeMin : change > changeMax ? changeMax : change;
            
            count += changeThisFrame;
            count = count < countMin ? countMin : count > countMax ? countMax : count;

            this.junkCountText.text = FormattingUtility.JunkCountToString(count);

            yield return null;
        }
    }
    
    
    #endregion
}
