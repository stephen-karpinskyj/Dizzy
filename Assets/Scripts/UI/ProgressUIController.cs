﻿using System.Collections;
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
    private MedalUIController noviceMedal;

    [SerializeField]
    private MedalUIController proMedal;

    [SerializeField]
    private Vector2 junkAddRateRange = new Vector2(295, 305);
    
    [SerializeField]
    private Color normalTimeColor;

    [SerializeField]
    private Color newTimeColor;

    [SerializeField]
    private float timeChangeHalfFlashDuration = 0.25f;

    [SerializeField]
    private float timeChangeFlashCount = 2;


    #endregion
    
    
    #region Properties
    
    
    public MedalUIController NoviceMedal
    {
        get { return this.noviceMedal; }
    }
    
    public MedalUIController ProMedal
    {
        get { return this.proMedal; }
    }
    
    
    #endregion


    #region Unity


    private void Awake()
    {
        Debug.Assert(this.bestTimeText);
        Debug.Assert(this.lastTimeText);
        Debug.Assert(this.runCountText);
        Debug.Assert(this.junkCountText);
        Debug.Assert(this.addedJunkCountText);
        Debug.Assert(this.noviceMedal);
        Debug.Assert(this.proMedal);
    }

    private void OnDisable()
    {
        this.StopAllCoroutines();
    }
    
    
    #endregion
    
    
    #region Events
    
    
    public void OnLevelStop(LevelData data)
    {
        var isTrial = data is TrialLevelData;
        
        this.noviceMedal.gameObject.SetActive(isTrial);
        this.proMedal.gameObject.SetActive(isTrial);
    }
    
    public void OnLevelLoad(LevelData data)
    {
        var trialData = data as TrialLevelData;
        
        if (trialData != null)
        {
            this.NoviceMedal.UpdateTime(trialData.NoviceTime);
            this.ProMedal.UpdateTime(trialData.ProTime);
        }
    }
    
    
    #endregion


    #region Public


    public void UpdateBestTime(float time, float diff)
    {
        this.StartCoroutine(this.TimeChangeCoroutine(this.bestTimeText, time));
        this.bestTimeDiffText.gameObject.SetActive(true);
        this.bestTimeDiffText.text = LevelState.TimeDiffToString(diff);
    }

    public void UpdateLastTime(float time)
    {
        this.StartCoroutine(this.TimeChangeCoroutine(this.lastTimeText, time));
    }

    public void UpdateRunCount(int count)
    {
        this.runCountText.text = LevelState.RunCountToString(count);
    }

    public void UpdateJunkCount(int count, int change)
    {
        this.StartCoroutine(this.JunkChangeCoroutine(count, change));
    }

    public void ForceUpdateAll(LevelState state, int junkCount)
    {
        this.bestTimeText.text = LevelState.TimeToString(state.BestTime);
        this.bestTimeText.color = this.normalTimeColor;
        this.bestTimeDiffText.gameObject.SetActive(false);
        this.lastTimeText.text = LevelState.TimeToString(state.LastTime);
        this.lastTimeText.color = this.normalTimeColor;
        this.runCountText.text = LevelState.RunCountToString(state.RunCount);
        this.noviceMedal.UpdateEarnt(state.NoviceMedalEarnt);
        this.proMedal.UpdateEarnt(state.ProMedalEarnt);
        
        this.junkCountText.text = StateManager.JunkCountToString(junkCount);
        this.addedJunkCountText.gameObject.SetActive(false);
    }
    
    
    #endregion


    #region Coroutines
    

    private IEnumerator TimeChangeCoroutine(Text text, float time)
    {
        text.text = LevelState.TimeToString(time);

        for (int i = 0; i < this.timeChangeFlashCount; i++)
        {
            text.color = this.newTimeColor;
            yield return new WaitForSeconds(this.timeChangeHalfFlashDuration);
            text.color = this.normalTimeColor;
            yield return new WaitForSeconds(this.timeChangeHalfFlashDuration);
        }
    }

    private IEnumerator JunkChangeCoroutine(int count, int change)
    {
        if (change == 0)
        {
            this.addedJunkCountText.gameObject.SetActive(false);
            yield break;
        }

        this.addedJunkCountText.gameObject.SetActive(true);
        this.addedJunkCountText.text = change.ToString("+#;-#");

        var finalCount = count;
        count -= change;

        var dir = (int)Mathf.Sign(change);
        var changeMin = (dir == 1) ? 0 : change;
        var changeMax = (dir == 1) ? change : 0;
        var countMin = (dir == 1) ? count : finalCount;
        var countMax = (dir == 1) ? finalCount : count;

        while (change != 0)
        {
            var r = Random.Range(this.junkAddRateRange.x, this.junkAddRateRange.y);
            var changeThisFrame = Mathf.RoundToInt(Time.deltaTime * r * dir);

            if (changeThisFrame == 0)
            {
                changeThisFrame = 1 * dir;
            }

            change = Mathf.Clamp(change - changeThisFrame, changeMin, changeMax);
            count = Mathf.Clamp(count + changeThisFrame, countMin, countMax);

            this.junkCountText.text = StateManager.JunkCountToString(count);

            yield return null;
        }
    }
    
    
    #endregion
}
