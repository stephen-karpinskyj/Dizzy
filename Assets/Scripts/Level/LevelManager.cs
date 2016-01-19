using System;
using System.Collections.Generic;
using UnityEngine;

using Object = UnityEngine.Object;

public class LevelManager : MonoBehaviour
{
    #region Fields
    
    
    private CanvasUIController canvas;
    
    private List<JunkController> uncollectedJunk;
    private Dictionary<LevelObjectNode, LevelObjectController> objectDic;
    private LevelObjectPool objectPool;
    
    private float startTime;
    
    private Action onLevelWin;
    private int earntJunk;
    
    
    #endregion
    
    
    #region Properties
    
    
    public LevelData CurrentLevel { get; private set; }
    public LevelState CurrentLevelState { get; private set; }
    
    
    #endregion
    
    
    #region Unity
    
    
    private void Awake()
    {
        this.uncollectedJunk = new List<JunkController>();
        this.objectDic = new Dictionary<LevelObjectNode, LevelObjectController>();
        this.objectPool = this.gameObject.AddComponent<LevelObjectPool>();
    }
    
    
    #endregion
    
    
    #region Events
    
    
    public void OnLevelLoad(CanvasUIController canvas, LevelData levelPrefab, Action onLevelWin)
    {
        this.canvas = canvas;
        
        if (this.CurrentLevel)
        {
            foreach (var kv in this.objectDic)
            {
                this.objectPool.Return(kv.Value);
            }
         
            this.objectDic.Clear();
            
            Object.Destroy(this.CurrentLevel.gameObject);
        }
        
        this.CurrentLevel = Object.Instantiate(levelPrefab);
        this.CurrentLevelState = StateManager.Instance.GetLevel(this.CurrentLevel.Id);
        this.onLevelWin = onLevelWin;
        
        this.canvas.Progress.NoviceMedal.UpdateTime(this.CurrentLevel.NoviceTime);
        this.canvas.Progress.ProMedal.UpdateTime(this.CurrentLevel.ProTime);
                
        foreach (var n in this.CurrentLevel.GetComponentsInChildren<JunkNode>())
        {
            this.objectDic[n] = this.objectPool.Get<JunkController>();
        }
    }
    
    public void OnLevelStop()
    {
        this.EarnCollectedJunk();
        this.uncollectedJunk.Clear();
        
        foreach (var kv in this.objectDic)
        {
            kv.Value.transform.parent = kv.Key.transform;
            kv.Value.transform.localPosition = Vector3.zero;
            kv.Value.transform.localRotation = Quaternion.identity;
            kv.Value.OnLevelStop(this.PickupObject);
            
            var junk = kv.Value as JunkController;
                
            if (junk)
            {
                this.uncollectedJunk.Add(junk);
            }
        }
    }
    
    public void OnLevelWin()
    {
        var prevBest = this.CurrentLevelState.BestTime;
        var prevLast = this.CurrentLevelState.LastTime;
        var prevRuns = this.CurrentLevelState.RunCount;
        var prevNovice = this.CurrentLevelState.NoviceMedalEarnt;
        var prevPro = this.CurrentLevelState.ProMedalEarnt;
        
        var currBest = this.CurrentLevelState.HandleWin(Time.time - this.startTime, this.CurrentLevel.NoviceTime, this.CurrentLevel.ProTime);
        var isNewTimeRecord = currBest < prevBest;
        var currLast = this.CurrentLevelState.LastTime;
        var currRuns = this.CurrentLevelState.RunCount;
        var currNovice = this.CurrentLevelState.NoviceMedalEarnt;
        var currPro = this.CurrentLevelState.ProMedalEarnt;

        this.canvas.HandleLevelWin(isNewTimeRecord);

        if (isNewTimeRecord)
        {
            this.canvas.Progress.UpdateBestTime(currBest, currBest - prevBest);
        }
        
        if (!Mathf.Approximately(prevLast, currLast))
        {
            this.canvas.Progress.UpdateLastTime(currLast);
        }
        
        if (prevRuns != currRuns)
        {
            this.canvas.Progress.UpdateRunCount(currRuns);
        }
        
        if (prevNovice != currNovice)
        {
            this.canvas.Progress.NoviceMedal.UpdateEarnt(currNovice);
        }
        
        if (prevPro != currPro)
        {
            this.canvas.Progress.ProMedal.UpdateEarnt(currPro);
        }
    }
    
    public void OnLevelStart()
    {
        foreach (var kv in this.objectDic)
        {
            kv.Value.OnLevelStart();
        }

        this.startTime = Time.time;
    }
    
    
    #endregion
    
    
    #region Private
    
    
    private void PickupObject(LevelObjectController obj)
    {
        var junk = obj as JunkController;
            
        if (junk)
        {
            this.uncollectedJunk.Remove(junk);
            this.earntJunk += junk.ChosenElement.JunkValue;

            if (this.uncollectedJunk.Count <= 0)
            {
                this.onLevelWin();
            }
        }
        else
        {
            throw new NotImplementedException();
        }
    }
    
    private void EarnCollectedJunk()
    {
        if (this.earntJunk == 0)
        {
            return;
        }
        
        var prev = StateManager.Instance.JunkCount;
        var curr = StateManager.Instance.HandleNewJunk(this.earntJunk);
        var diff = curr - prev;
        
        if (diff != 0)
        {
            this.canvas.Progress.UpdateJunkCount(curr, curr - prev);
        }
        
        this.earntJunk = 0;
    }
    
    
    #endregion
}
