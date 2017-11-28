using System;
using System.Collections;
using UnityEngine;

using Random = UnityEngine.Random;

public abstract class LevelObjectController : MonoBehaviour
{
    [SerializeField]
    private Vector2 showPositionRange = new Vector2(-4f, 4f);

    [SerializeField]
    private float maxShowDuration = 0.1f;

    [SerializeField]
    private Vector2 showDurationOffset = new Vector2(0f, 0.05f);

    [SerializeField]
    private AnimationCurve scaleCurve;

    [SerializeField]
    private float scaleDuration = 0.1f;
    
    private Vector3 initialScale;

    protected virtual void Awake()
    {
        this.initialScale = this.transform.localScale;
    }

    public virtual void OnLevelStop() { }
    public virtual void OnLevelStart() { }
    
    
    #region Coroutines


    protected IEnumerator ShowCoroutine(GameObject go, Action onComplete = null)
    {
        var x = this.transform.position.x - this.showPositionRange.x;
        var t = x / (this.showPositionRange.y - this.showPositionRange.x);

        yield return new WaitForSeconds(this.maxShowDuration * t + Random.Range(this.showDurationOffset.x, this.showDurationOffset.y));

        this.transform.localScale = this.initialScale;
        go.SetActive(true);

        var time = Time.time;
        var progress = 0f;

        while (progress < 1f) 
        {
            progress = Mathf.Clamp01((Time.time - time) / this.scaleDuration);
            this.transform.localScale = this.initialScale * this.scaleCurve.Evaluate(progress);
            yield return null;
        }
        
        if (onComplete != null)
        {
            onComplete();
        }
    }


    #endregion
}

