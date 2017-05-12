using System.Collections;
using UnityEngine;

public class StarfieldBlast : MonoBehaviour
{
    private const string BlastPropertiesProperty = "_Blast";
    private const string BlastColourProperty = "_BlastColor";
    
    [SerializeField]
    private Material mat;
    
    [SerializeField]
    private Color color;
    
    [SerializeField]
    private AnimationCurve radiusCurve;
    
    [SerializeField]
    private Vector2 heightRange = new Vector2(0, 0.1f);
    
    [SerializeField]
    private AnimationCurve heightCurve;
    
    private static bool isPlaying;
    
    private void Awake()
    {
        this.UpdateColor(this.color);
    }
        
    private void UpdateColor(Color color)
    {
        var multiplier = color.a * 5f;
        this.mat.SetVector(BlastColourProperty, new Vector4(color.r * multiplier, color.g * multiplier, color.b * multiplier, 1f));
    }
    
    private void UpdateProperties(Vector2 screenPosition, float radius, float height)
    {
        this.mat.SetVector(BlastPropertiesProperty, new Vector4(screenPosition.x, screenPosition.y, radius - 1f, height));
    }
    
    public void Play(float duration, Vector2 radiusRange)
    {
        if (isPlaying)
        {
            this.ResetValues();
        }
        
        isPlaying = true;
        this.StartCoroutine(this.PlayCoroutine(duration, radiusRange));
    }
    
    private IEnumerator PlayCoroutine(float duration, Vector2 radiusRange)
    {        
        var startTime = Time.time;
        var percentage = 0f;
        
        while (percentage < 1f)
        {
            percentage = Mathf.Clamp01((Time.time - startTime) / duration);
                        
            var screenPos = (Vector2)this.transform.position;
            var radius = Mathf.Lerp(radiusRange.x, radiusRange.y, this.radiusCurve.Evaluate(percentage));
            var height = Mathf.Lerp(this.heightRange.x, this.heightRange.y, this.heightCurve.Evaluate(percentage));
            
            this.UpdateProperties(screenPos, radius, height);
            
            yield return null;
        }
        
        this.ResetValues();
    }
    
    private void ResetValues()
    {
        isPlaying = false;
        this.UpdateProperties(Vector2.zero, -1f, 0f);
    }
}
