using UnityEngine;
using System.Collections;

public class Nebula : MonoBehaviour
{
    private const string PropertyName = "_EdgeMaskFalloff";
    
    public float speed;
    
    [SerializeField]
    private bool canFlicker;
    
    public Vector2 flickerBrightnessMultiplerRange = new Vector2(1.3f, 2f);
    public int maxFlickers = 4;
    public Vector2 flickerGapDurationRange = new Vector2(2f, 5f);
    public Vector2 flickerDurationRange = new Vector2(0.3f, 0.7f);
    public AnimationCurve[] flickerCurves;
    
    [SerializeField]
    private Material nebulaMaterial;

    private float nextFlickerTime;
    private bool isFlickering;
    private float initialNebulaBrightness;

	void Awake()
    {
        //this.transform.rotation = Random.rotation;
        
        if (this.canFlicker)
        {
            this.isFlickering = false;
            this.GenerateNextFlickerTime();
            this.initialNebulaBrightness = this.nebulaMaterial.GetFloat(PropertyName);
        }
	}
	
	void Update()
    {
        this.transform.rotation *= Quaternion.Euler(0, this.speed * Time.deltaTime, 0);
        
        if (this.canFlicker && !this.isFlickering)
        {
            if (Time.time >= this.nextFlickerTime)
            {
                this.StartCoroutine(this.FlickerCoroutine());
            }
        }
	}
    
    private void GenerateNextFlickerTime()
    {
        this.nextFlickerTime = Time.time + Random.Range(this.flickerGapDurationRange.x, this.flickerGapDurationRange.y);
    }
    
    private IEnumerator FlickerCoroutine()
    {
        var numFlickers = Random.Range(1, this.maxFlickers);
        
        for (var i = 0; i < numFlickers; i++)
        {
            this.isFlickering = true;
            
            var startTime = Time.time;
            var duration = Random.Range(this.flickerDurationRange.x, this.flickerDurationRange.y);
            var curve = this.flickerCurves[Random.Range(0, this.flickerCurves.Length - 1)];
            
            while (this.isFlickering)
            {
                var timeSoFar = Time.time - startTime;
                var target = this.initialNebulaBrightness * (1f / Random.Range(this.flickerBrightnessMultiplerRange.x, this.flickerBrightnessMultiplerRange.y));
                var v = Mathf.Lerp(this.initialNebulaBrightness, target, curve.Evaluate(timeSoFar / duration));
                
                this.nebulaMaterial.SetFloat(PropertyName, v);
                
                yield return null;
                
                if (timeSoFar >= duration)
                {
                    this.isFlickering = false;
                }
            }
            
            this.nebulaMaterial.SetFloat(PropertyName, this.initialNebulaBrightness);
        }
        
        this.GenerateNextFlickerTime();
    }
}
