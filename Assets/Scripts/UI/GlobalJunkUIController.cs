using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GlobalJunkUIController : MonoBehaviour
{
    [SerializeField]
    private Text countText;

    [SerializeField]
    private Text addedCountText;
    
    [SerializeField]
    private Text multiplierText;
    
    [SerializeField]
    private Text addedMultiplierText;

    [SerializeField]
    private int addRate = 10;

    private void Awake()
    {
        Debug.Assert(this.countText);
        Debug.Assert(this.addedCountText);
        Debug.Assert(this.multiplierText);
        Debug.Assert(this.addedMultiplierText);
    }

    public void UpdateCount(ulong count, ulong change)
    {
        this.StartCoroutine(this.JunkChangeCoroutine(count, change));
    }
    
    public void UpdateMultiplier(float multiplier, float change = 0f)
    {
        this.multiplierText.text = FormattingUtility.JunkMultiplierToString(multiplier);
        
        if (Mathf.Approximately(change, 0f))
        {
            this.addedMultiplierText.text = string.Empty;
        }
        else
        {
            this.addedMultiplierText.text = FormattingUtility.SignedJunkMultiplierToString(change);
        }
    }

    public void ForceUpdateAll(ulong count, float multiplier)
    {
        this.countText.text = FormattingUtility.JunkCountToString(count);
        this.addedCountText.gameObject.SetActive(false);
        
        this.UpdateMultiplier(multiplier);
    }

    private IEnumerator JunkChangeCoroutine(ulong count, ulong change)
    {
        if (change == 0)
        {
            this.addedCountText.gameObject.SetActive(false);
            yield break;
        }

        this.addedCountText.gameObject.SetActive(true);
        this.addedCountText.text = FormattingUtility.SignedJunkCountToString(change);

        var finalCount = count;
        count -= change;

        var dir = (int)Mathf.Sign(change);
        var changeMin = (dir == 1) ? 0 : change;
        var changeMax = (dir == 1) ? change : 0;
        var countMin = (dir == 1) ? count : finalCount;
        var countMax = (dir == 1) ? finalCount : count;

        var range = Math.Pow(this.addRate, Math.Max(1, Math.Log10(count + change) - 0.5));

        while (count != finalCount)
        {
            var changeThisFrame = (ulong)(Time.deltaTime * range * dir);

            if (changeThisFrame == 0)
            {
                changeThisFrame = (ulong)(1 * dir);
            }

            change -= changeThisFrame;
            change = change < changeMin ? changeMin : change > changeMax ? changeMax : change;
            
            count += changeThisFrame;
            count = count < countMin ? countMin : count > countMax ? countMax : count;

            this.countText.text = FormattingUtility.JunkCountToString(count);

            yield return null;
        }
    }
}
