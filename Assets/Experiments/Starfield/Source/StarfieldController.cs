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
    
    [SerializeField]
    private float nebulaDistance = 20f;
    
    [SerializeField]
    private float farStarDistance = 3f;
    
    [SerializeField]
    private float midStarDistance = 2f;
    
    [SerializeField]
    private float nearStarDistance = 0.5f;
        
	private StarfieldPoint[] points;
    
    private Vector3 initPosition;

	private void Start()
    {    
        this.SetNebulaPointCount(this.numPoints);
        
        this.points = new StarfieldPoint[this.numPoints];

		for (var i = 0; i < this.points.Length; i++)
        {
            var pos = new Vector2(Random.Range(this.bounds.x, this.bounds.y), Random.Range(this.bounds.z, this.bounds.w));
            var vel = new Vector2(Random.Range(this.velocityRange.x, this.velocityRange.y), Random.Range(this.velocityRange.x, this.velocityRange.y));
            var radius = Random.Range(this.radiusRange.x, this.radiusRange.y);
			var colour = new Vector3 (Random.Range (0f, 1f), Random.Range (0f, .5f), Random.Range (0f, 1f));
            
			var p = new StarfieldPoint(pos, vel, radius, this.intensity, colour);
            p.UpdateOffset(Vector2.zero);
			this.material.SetVector("_Colour" + i, p.Colour);
			this.material.SetVector("_Points" + i, p.Properties);
            
            this.points[i] = p;
		}
        
        this.UpdateOffset(Vector2.zero);
	}
    
    private void SetNebulaPointCount(int count)
    { 
		this.material.SetInt("_Points_Length", count);
    }
    
    public void ToggleNebula(bool show)
    {
        if (show)
        {
            this.SetNebulaPointCount(this.numPoints);
        }
        else
        {
            this.SetNebulaPointCount(0);
        }
    }
    
    public void ResetPosition()
    {
        if (this.initPosition == Vector3.zero)
        {
            this.initPosition = this.transform.position;
        }
        
        this.transform.position = this.initPosition;
    }
    
	public void UpdateOffset(Vector2 offset)   
    {
        offset *= -this.speedMultiplier;
        
        var starFarOffset = offset * 1/this.farStarDistance;
        var starMidOffset = offset * 1/this.midStarDistance;
        var starNearOffset = offset * 1/this.nearStarDistance;
        
        var nebulaUVOffset = offset * this.nebulaDistance;
        var nebulaPosOffset = -nebulaUVOffset;
        
        this.material.SetVector("_Offset12", new Vector4(starFarOffset.x, starFarOffset.y, starMidOffset.x, starMidOffset.y));
        this.material.SetVector("_Offset34", new Vector4(starNearOffset.x, starNearOffset.y, nebulaUVOffset.x, nebulaUVOffset.y));
        
		for (var i = 0; i < this.points.Length; i++)
        {
            var p = this.points[i];
            p.UpdateOffset(nebulaPosOffset);
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
