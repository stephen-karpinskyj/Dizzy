using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ProgressUIController : MonoBehaviour
{
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
    private int junkAddPerSecond = 100;
    
    [SerializeField]
    private Color normalTimeColor;

    [SerializeField]
    private Color newTimeColor;

    [SerializeField]
    private float timeChangeHalfFlashDuration = 0.25f;

    [SerializeField]
    private float timeChangeFlashCount = 2;

    private void Awake()
    {
        Debug.Assert(this.bestTimeText);
        Debug.Assert(this.lastTimeText);
        Debug.Assert(this.runCountText);
        Debug.Assert(this.junkCountText);
        Debug.Assert(this.addedJunkCountText);
        Debug.Assert(this.noviceMedal);
        Debug.Assert(this.proMedal);

        this.ForceUpdateAll();

        StateManager.Instance.OnBestTimeChange += this.HandleBestTimeChange;
        StateManager.Instance.OnLastTimeChange += this.HandleLastTimeChange;
        StateManager.Instance.OnRunCountChange += this.HandleRunCountChange;
        StateManager.Instance.OnJunkCountChange += this.HandleJunkCountChange;
        StateManager.Instance.OnNoviceMedalEarntChange += this.HandleNoviceMedalEarntChange;
        StateManager.Instance.OnProMedalEarntChange += this.HandleProMedalEarntChange;
    }

    private void OnDisable()
    {
        this.StopAllCoroutines();

        if (!StateManager.Exists)
        {
            return;
        }

        this.ForceUpdateAll();
    }

    private void HandleBestTimeChange(float time, float diff)
    {
        this.StartCoroutine(this.TimeChangeCoroutine(this.bestTimeText, time));
        this.bestTimeDiffText.gameObject.SetActive(true);
        this.bestTimeDiffText.text = StateManager.TimeDiffToString(diff);
    }

    private void HandleLastTimeChange(float time)
    {
        this.StartCoroutine(this.TimeChangeCoroutine(this.lastTimeText, time));
    }

    private void HandleRunCountChange(int count)
    {
        this.runCountText.text = StateManager.RunCountToString(count);
    }

    private void HandleJunkCountChange(int count, int change)
    {
        this.StartCoroutine(this.JunkChangeCoroutine(count, change));
    }

    private void HandleNoviceMedalEarntChange(bool earnt)
    {
        this.noviceMedal.SetEarnt(earnt, false);
    }

    private void HandleProMedalEarntChange(bool earnt)
    {
        this.proMedal.SetEarnt(earnt, false);
    }

    private IEnumerator TimeChangeCoroutine(Text text, float time)
    {
        text.text = StateManager.TimeToString(time);

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
            var changeThisFrame = Mathf.RoundToInt(Time.deltaTime * this.junkAddPerSecond * dir);

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

    private void ForceUpdateAll()
    {
        this.bestTimeText.text = StateManager.TimeToString(StateManager.Instance.BestTime);
        this.bestTimeText.color = this.normalTimeColor;
        this.bestTimeDiffText.gameObject.SetActive(false);
        this.lastTimeText.text = StateManager.TimeToString(StateManager.Instance.LastTime);
        this.lastTimeText.color = this.normalTimeColor;
        this.runCountText.text = StateManager.RunCountToString(StateManager.Instance.RunCount);
        this.junkCountText.text = StateManager.JunkCountToString(StateManager.Instance.JunkCount);
        this.addedJunkCountText.gameObject.SetActive(false);
        this.noviceMedal.SetEarnt(StateManager.Instance.NoviceMedalEarnt, true);
        this.proMedal.SetEarnt(StateManager.Instance.ProMedalEarnt, true);
    }
}
