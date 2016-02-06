using System;
using System.Collections.Generic;
using UnityEngine;

using Object = UnityEngine.Object;

public class LevelManager : MonoBehaviour
{
    #region Fields
    
    
    private CanvasUIController canvas;
        
    private HashSet<LevelObjectNode> nodes;
    private LevelObjectPool objectPool;
    
    private HashSet<JunkController> allJunk;
    private HashSet<JunkController> uncollectedJunk;
    private int totalCollectedJunkValue;
    
    private float startTime;
    private Action onLevelWin;
    
    
    #endregion
    
    
    #region Properties
    
    
    public LevelData CurrentLevel { get; private set; }
    public LevelState CurrentLevelState { get; private set; }
    
    
    #endregion
    
    
    #region Unity
    
    
    private void Awake()
    {
        this.nodes = new HashSet<LevelObjectNode>();
        this.objectPool = this.gameObject.AddComponent<LevelObjectPool>();
        
        this.allJunk = new HashSet<JunkController>();
        this.uncollectedJunk = new HashSet<JunkController>();
    }
    
    
    #endregion
    
    
    #region Events
    
    
    public void OnGameStart(CanvasUIController canvas)
    {
        this.canvas = canvas;
    }
    
    public void OnLeveUnload()
    {
        if (!this.CurrentLevel)
        {
            return;
        }
        
        foreach (var n in this.nodes)
        {
            n.OnLevelUnload(this.objectPool);
        }
        
        this.nodes.Clear();
        this.allJunk.Clear();
        
        Object.Destroy(this.CurrentLevel.gameObject);
        this.CurrentLevel = null;
    }
    
    public void OnLevelLoad(LevelData levelPrefab, Action onLevelWin)
    {
        Debug.Assert(!this.CurrentLevel);
        
        this.CurrentLevel = Object.Instantiate(levelPrefab);
        this.CurrentLevelState = StateManager.Instance.GetLevel(this.CurrentLevel.Id);
        this.onLevelWin = onLevelWin;
                
        foreach (var n in this.CurrentLevel.GetComponentsInChildren<LevelObjectNode>())
        {
            var result = n.OnLevelLoad(this.objectPool);
            
            if (result.Junk != null)
            {
                this.allJunk.UnionWith(result.Junk);
            }
            
            this.nodes.Add(n);
        }
        
        foreach (var j in this.allJunk)
        {
            j.Initialize(this.OnJunkCollect);
        }
    }
    
    public void OnLevelStop()
    {
        this.EarnCollectedJunk();
        
        foreach (var n in this.nodes)
        {
            n.OnLevelStop();
        }
        
        this.uncollectedJunk.Clear();
        this.uncollectedJunk.UnionWith(allJunk);
    }
    
    public void OnLevelWin()
    {
        if (this.CurrentLevel is TrialLevelData)
        {
            this.HandleTrialWin();
        }
    }
    
    public void OnLevelStart()
    {
        foreach (var n in this.nodes)
        {
            n.OnLevelStart();
        }

        this.startTime = Time.time;
    }
    
    
    #endregion
    
    
    #region Private
    
    
    private void OnJunkCollect(JunkController junk)
    {
        Debug.Assert(junk);
        
        this.totalCollectedJunkValue += junk.ChosenElement.JunkValue;
        this.uncollectedJunk.Remove(junk);
        
        if (this.uncollectedJunk.Count <= 0)
        {
            this.onLevelWin();
        }
    }
    
    private void EarnCollectedJunk()
    {
        if (this.totalCollectedJunkValue == 0)
        {
            return;
        }
        
        var prev = StateManager.Instance.JunkCount;
        var curr = StateManager.Instance.HandleNewJunk(this.totalCollectedJunkValue);
        var diff = curr - prev;
        
        if (diff != 0)
        {
            this.canvas.Progress.UpdateJunkCount(curr, curr - prev);
        }
        
        this.totalCollectedJunkValue = 0;
    }
    
    
    private void HandleTrialWin()
    {
        var prevBest = this.CurrentLevelState.BestTime;
        var prevLast = this.CurrentLevelState.LastTime;
        var prevRuns = this.CurrentLevelState.RunCount;
        var prevNovice = this.CurrentLevelState.NoviceMedalEarnt;
        var prevPro = this.CurrentLevelState.ProMedalEarnt;

        var currBest = this.CurrentLevelState.HandleWin(Time.time - this.startTime, this.CurrentLevel);
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


    #endregion
}
