using UnityEngine;
using UnityEngine.UI;

public class TimerUIController : MonoBehaviour
{
    [SerializeField]
    private Text text;
    
    [SerializeField]
    private Color expiredColor;
    
    [SerializeField]
    private Color alertColor;
    
    [SerializeField]
    private Color defaultColor;
    
    [SerializeField]
    private float alertTime = 3f;
    
    private float startTime;
    private float bestTime;
    private float currentTime;
    
    public void StartTimer(float bestTime)
    {
        this.startTime = Time.time;
        this.bestTime = bestTime;
        this.currentTime = bestTime;
        
        this.UpdateTimer(bestTime);
        this.gameObject.SetActive(true);
    }
    
    public void StopTimer()
    {
        this.gameObject.SetActive(false);
    }
    
    private void UpdateTimer(float time)
    {
        if (time < 0f)
        {
            this.text.color = this.expiredColor;
            this.text.text = ":-(";
        }
        else if (time < this.alertTime)
        {
            this.text.color = Mathf.Abs(time - (int)time) < 0.5f ? this.defaultColor : this.alertColor;
            this.text.text = string.Format("{0:f2}", time);
        }
        else
        {
            this.text.color = this.defaultColor;
            this.text.text = string.Format("{0:f1}", time);
        }
    }
    
    private void Update()
    {
        this.currentTime = bestTime - (Time.time - this.startTime);
        this.UpdateTimer(this.currentTime);
    }
}
