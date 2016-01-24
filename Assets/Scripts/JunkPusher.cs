using System.Collections;
using UnityEngine;

public class JunkPusher : MonoBehaviour
{
    private JunkController junk;
    private float duration;
    private float distance;
    private AnimationCurve curve;
        
    public void Initialise(JunkController junk, float rotation, float duration, float distance, AnimationCurve curve)
    {
        this.junk = junk;
        this.transform.eulerAngles = Vector3.forward * rotation;
        this.duration = duration;
        this.distance = distance;
        this.curve = curve;
    }
    
    public void StartRunning()
    {
        this.StartCoroutine(this.RunCoroutine());
    }
    
    public void StopRunning()
    {
        this.StopAllCoroutines();
    }
    
    private IEnumerator RunCoroutine()
    {
        var timeLeft = this.duration;
        var isJunkRunning = false;
        
        while (timeLeft > 0)
        {
            var curveValue = 1f - (timeLeft / this.duration);
            var newY = this.curve.Evaluate(curveValue) * this.distance;
            
            var newPos = this.junk.transform.localPosition;
            newPos.y = newY;
            this.junk.transform.localPosition = newPos;
            
            if (curveValue > 0.25f && !isJunkRunning)
            {
                junk.IsAttractable = true;
            }
            
            yield return null;
            
            if (junk.IsAttracted)
            {
                break;
            }
            
            timeLeft = Mathf.Clamp(timeLeft - Time.deltaTime, 0f, this.duration);
        }
    }
}
