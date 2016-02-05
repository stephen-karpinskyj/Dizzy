using UnityEngine;

public class BouncingPoints : MonoBehaviour
{
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

	private void Awake()
    {
		this.material.SetInt("_Points_Length", this.numPoints);

        this.points = new StarfieldPoint[this.numPoints];

		for (var i = 0; i < this.points.Length; i++)
        {
            var pos = new Vector2(Random.Range(this.bounds.x, this.bounds.y), Random.Range(this.bounds.z, this.bounds.w));
            var vel = new Vector2(Random.Range(this.velocityRange.x, this.velocityRange.y), Random.Range(this.velocityRange.x, this.velocityRange.y));
            var radius = Random.Range(this.radiusRange.x, this.radiusRange.y);
			var colour = new Vector3 (Random.Range (0f, 1f), Random.Range (0f, 1f), Random.Range (0f, 1f));
            
			this.points[i] = new StarfieldPoint(pos, vel, radius, this.intensity, colour);
		}
	}
	
	private void Update()
    {
		for (var i = 0; i < this.points.Length; i++)
        {
			var p = this.points[i];
            
            p.Update(Time.deltaTime, this.bounds);
            
			material.SetVector("_Points" + i, p.Position);
			material.SetVector("_Properties" + i, p.Properties);
			material.SetVector ("_Colour" + i, p.Colour);
		}
	}
    
    private void OnDrawGizmos()
    {
        if (this.points == null)
        {
            return;
        }
        
        foreach (var p in this.points)
        {
            Gizmos.DrawSphere(p.Position, p.Radius);
        }
    }
}
