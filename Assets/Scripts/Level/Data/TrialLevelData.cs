using System.Collections.Generic;
using UnityEngine;

public class TrialLevelData : LevelData
{
    [SerializeField]
    private TrialGoalData[] goals;
    
    public float GetCompletedGoalMultiplier(float time)
    {
        var multiplier = 0f;
        
        foreach (var g in this.goals)
        {
            if (g.Time >= time)
            {
                multiplier += g.MultiplierIncrease;
            }
        }
        
        return multiplier;
    }
    
    public List<TrialGoalData> GetActiveGoals(float time, int count)
    {
        var active = new List<TrialGoalData>();
        
        foreach (var g in this.goals)
        {
            if (g.Time < time)
            {
                active.Add(g);
            }
            
            if (active.Count >= count)
            {
                break;
            }
        }
        
        return active;
    }
}
