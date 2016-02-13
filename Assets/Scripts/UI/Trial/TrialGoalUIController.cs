using UnityEngine;
using UnityEngine.UI;

public class TrialGoalUIController : MonoBehaviour
{
    [SerializeField]
    private Text timeText;
    
    [SerializeField]
    private Text multiplierIncreaseText;
    
    public void UpdateData(TrialGoalData goal)
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
