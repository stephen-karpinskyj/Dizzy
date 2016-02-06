using UnityEngine;
using System.Collections;

public class LaunchLightsController : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer[] lights;

    [SerializeField]
    private Color dimColour;

    [SerializeField]
    private Color litColour;

    [SerializeField]
    private float hideDelay = 0.2f;

    [SerializeField]
    private float perLightDelay = 0.2f;

    [SerializeField]
    private float perCycleDelay = 0.5f;

    private bool isRunning = false;

    private void Start()
    {
        foreach (var l in this.lights)
        {
            l.color = this.dimColour;
        }
    }

    private void LightUp()
    {
        if (this.isRunning)
        {
            return;
        }

        this.isRunning = true;
        
        this.StartCoroutine(this.LightCoroutine());
    }
    
    private void ForceHide()
    {
        foreach (var l in this.lights)
        {
            l.enabled = false;
        }
    }

    private IEnumerator HideCoroutine()
    {
        if (!this.isRunning)
        {
            yield break;
        }

        this.isRunning = false;

        yield return new WaitForSeconds(this.hideDelay);

        this.ForceHide();

        this.StopAllCoroutines();
    }

    private IEnumerator LightCoroutine()
    {
        foreach (var l in this.lights)
        {
            l.enabled = true;
        }

        while (true)
        {
            foreach (var l in this.lights)
            {
                l.color = this.litColour;
                yield return new WaitForSeconds(this.perLightDelay);
                l.color = this.dimColour;
            }

            yield return new WaitForSeconds(this.perCycleDelay);
        }
    }


    #region Level events


    public void OnLevelStop(bool show)
    {
        if (show)
        {
            this.LightUp();
        }
        else
        {
            this.ForceHide();
        }
    }

    public void OnLevelStart()
    {
        this.StartCoroutine(HideCoroutine());
    }


    #endregion
}
