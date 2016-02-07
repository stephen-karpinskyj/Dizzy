using UnityEngine;
using UnityEngine.UI;

public class TrialLevelGoalUIController : MonoBehaviour
{
    [SerializeField]
    private Text timeText;
    
    [SerializeField]
    private Text multiplierIncreaseText;
    
    public Text TimeText
    {
        get { return this.timeText; }
    }
    
    public void UpdateData(TrialLevelDataGoal goal)
    {
        this.timeText.text = FormattingUtility.VariableDPTimeToString(goal.Time);
        this.multiplierIncreaseText.text = FormattingUtility.SignedJunkMultiplierToString(goal.MultiplierIncrease);
        this.gameObject.SetActive(true);
    }
    
    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
