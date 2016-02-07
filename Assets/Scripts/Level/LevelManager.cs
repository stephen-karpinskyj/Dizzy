using System;
using System.Collections.Generic;
using UnityEngine;

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour
{
    #region Fields
    
    
    private CanvasUIController canvas;
        
    private HashSet<LevelObjectNode> nodes;
    private LevelObjectPool objectPool;
    
    private HashSet<JunkController> allJunk;
    private HashSet<JunkController> uncollectedJunk;
    private ulong totalCollectedJunkValue;
    
    private float startTime;
    private Action onLevelWin;
    
    
    #endregion
    
    
    #region Properties
    
    
    public LevelData CurrentLevel { get; private set; }
    public TrialLevelState CurrentLevelState { get; private set; }
    
    
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
        
        if (this.CurrentLevel is TrialLevelData)
        {
            this.CurrentLevelState = StateManager.Instance.GetLevel(this.CurrentLevel.Id);
            this.CurrentLevelState.UpdateWithData(this.CurrentLevel as TrialLevelData);
        }
        else
        {
            this.CurrentLevelState = null;
        }
        
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
        
        this.InitialiseJunk();
        
        foreach (var n in this.nodes)
        {
            n.OnLevelStop();
        }
    }
    
    public void ReinitialiseJunkValues()
    {
        this.InitialiseJunk(true);
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
        
        this.totalCollectedJunkValue += junk.ChosenElement.Value;
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

        var currBest = this.CurrentLevelState.HandleWin(Time.time - this.startTime, this.CurrentLevel as TrialLevelData);
        var isNewTimeRecord = currBest < prevBest;
        var currLast = this.CurrentLevelState.LastTime;
        var currRuns = this.CurrentLevelState.RunCount;
        
        var prevMultiplier = StateManager.Instance.JunkMultiplier;

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
        
        this.canvas.Progress.UpdateGoals(this.CurrentLevelState);
        
        this.CurrentLevelState.UpdateWithData(this.CurrentLevel as TrialLevelData);
        StateManager.Instance.UpdateJunkMultiplier();
        
        var currMultiplier = StateManager.Instance.JunkMultiplier;
        this.canvas.Progress.UpdateJunkMultiplier(currMultiplier, currMultiplier - prevMultiplier);
        
        // SK: HACK: Shouldn't need to call this twice (both in level stop and level won)
        this.InitialiseJunk();
    }
    
    private void InitialiseJunk(bool showJunkNow = false)
    {
        this.uncollectedJunk.Clear();
        this.uncollectedJunk.UnionWith(allJunk);
        
        var total = Math.Round((ulong)this.allJunk.Count * (double)StateManager.Instance.JunkMultiplier);
        
        var shuffledJunk = new List<JunkController>(this.allJunk);
        shuffledJunk.Shuffle();
        
        var highValueDic = new Dictionary<int, double>();
        var numAssigned = 0;
        
        // Calculate high-value element counts
        {
            var temp = total;
            var log10 = (int)Math.Round(Math.Log10(total), MidpointRounding.AwayFromZero);
            
            const int NumHighValueElementTypes = 2;
            for (var i = log10 - 1; i >= Mathf.Max(log10 - NumHighValueElementTypes, 1); i--)
            {
                var scale = Math.Pow(10, i);
                var floor = Math.Floor(temp / scale);
                var ceil = Math.Ceiling(temp / scale);
                
                temp -= scale * floor;
                highValueDic[i] = ceil;
            }
        }
        
        // Assign high-value elements
        foreach (var kv in highValueDic)
        {
            var exponent = kv.Key;
            
            for (var i = 0; i < kv.Value; i++)
            {
                shuffledJunk[numAssigned].SetElement(exponent, showJunkNow);
                numAssigned++;
            }
        }
        
        // Assign low-value elements
        {
            var maxLeftoverExponent = total > 1000 ? 1 : 0;
            
            while (numAssigned < shuffledJunk.Count)
            {
                var exponent = maxLeftoverExponent == 0 ? 0 : Random.Range(0, maxLeftoverExponent);
                
                shuffledJunk[numAssigned].SetElement(exponent, showJunkNow);
                numAssigned++;
            }
        }
    }


    #endregion
}
