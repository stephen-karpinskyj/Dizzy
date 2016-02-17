using System.Collections;
using UnityEngine;

public class TweenAlpha : MonoBehaviour
{
    [SerializeField]
    private AnimationCurve curve;
    
    [SerializeField]
    private float duration;
    
    [SerializeField]
    private float loopDelay;
    
    [SerializeField]
    private Renderer rend;
    
    [SerializeField]
    private string colourPropertyName = "_Color";
    
    public void Play()
    {
        this.StartCoroutine(this.PlayCoroutine());
    }
    
    private IEnumerator PlayCoroutine()
    {
	   while (true)
       {
           var startTime = Time.time;
           var progress = 0f;
           
           while (progress < 1f)
           {
               progress = Mathf.Clamp01((Time.time - startTime) / this.duration);
               var alpha = this.curve.Evaluate(progress);
               this.UpdateAlpha(alpha);
               
               yield return null;
           }
           
           yield return new WaitForSeconds(this.loopDelay);
       }
    }
    
    private void UpdateAlpha(float alpha)
    {
        var c = this.rend.material.GetColor(this.colourPropertyName);
        c.a = alpha;
        this.rend.material.SetColor(this.colourPropertyName, c);
    }
}
