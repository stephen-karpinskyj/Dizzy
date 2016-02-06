using UnityEngine;

public class StarfieldController : MonoBehaviour
{
    [SerializeField]
    private float speedMultiplier = 0.1f;
    
    [SerializeField, RangeAttribute(1, 99)]
    private int numPoints = 20;
    
    [SerializeField]
	private Material material;
    
    [SerializeField]
    private Vector2 velocityRange = new Vector2(-0.3f, 0.3f);
    
    [SerializeField]
    private Vector2 radiusRange = new Vector2(0.1f, 0.3f);
    
    [SerializeField]
    private float intensity = 1f;
    
    [SerializeField]
    private Vector4 bounds = new Vector4(-0.8f, 0.8f, -1f, 1f);
        
	private StarfieldPoint[] points;
	
    private Vector2 starFarOffset;
    private Vector2 starMidOffset;
    private Vector2 starNearOffset;
    
    private Vector2 nebulaFarOffset;
    private Vector2 nebulaNearOffset;
    
    private Vector3 initPosition;

    private void Awake()
    {
        this.initPosition = this.transform.position;
    }

	private void Start()
    {        
		this.material.SetInt("_Points_Length", this.numPoints);

        this.points = new StarfieldPoint[this.numPoints];

		for (var i = 0; i < this.points.Length; i++)
        {
            var pos = new Vector2(Random.Range(this.bounds.x, this.bounds.y), Random.Range(this.bounds.z, this.bounds.w));
            var vel = new Vector2(Random.Range(this.velocityRange.x, this.velocityRange.y), Random.Range(this.velocityRange.x, this.velocityRange.y));
            var radius = Random.Range(this.radiusRange.x, this.radiusRange.y);
			var colour = new Vector3 (Random.Range (0f, 1f), Random.Range (0f, .5f), Random.Range (0f, 1f));
            
			var p = new StarfieldPoint(pos, vel, radius, this.intensity, colour);
			this.material.SetVector("_Colour" + i, p.Colour);
            
            this.points[i] = p;
		}
        
        this.UpdateOffset(Vector2.zero);
	}
    
    public void ResetPosition()
    {
        this.transform.position = this.initPosition;
    }
    
	public void UpdateOffset(Vector2 offset)
    {
        offset *= -this.speedMultiplier;
        
        this.starFarOffset = offset;
        this.starMidOffset = offset * 2f;
        this.starNearOffset = offset * 3f;
        
        this.material.SetVector("_Offset12", new Vector4(this.starFarOffset.x, this.starFarOffset.y, this.starMidOffset.x, this.starMidOffset.y));
        this.material.SetVector("_Offset34", new Vector4(this.starNearOffset.x, this.starNearOffset.y, 0f, 0f));
        
        this.nebulaFarOffset = offset * 1f;
        this.nebulaNearOffset = offset * 3f;
        
		for (var i = 0; i < this.points.Length; i++)
        {            
			var p = this.points[i];
            
            //p.UpdateBounce(Time.deltaTime, this.bounds);
            p.UpdateOffset(i % 3 == 0 ? this.nebulaFarOffset : this.nebulaNearOffset);
            
			this.material.SetVector("_Points" + i, p.Properties);
		}
	}
    
    private void OnDrawGizmosSelected()
    {
        if (this.points == null)
        {
            return;
        }
        
        foreach (var p in this.points)
        {
            Gizmos.DrawWireSphere(new Vector2(p.PositionX, p.PositionY), p.Radius);
        }
    }
}
