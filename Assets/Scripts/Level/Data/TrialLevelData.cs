using System.Collections.Generic;
using UnityEngine;

public class TrialLevelData : LevelData
{
    [SerializeField]
    private TrialLevelDataGoal[] goals;
    
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
    
    public List<TrialLevelDataGoal> GetActiveGoals(float time, int count)
    {
        var active = new List<TrialLevelDataGoal>();
        
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
