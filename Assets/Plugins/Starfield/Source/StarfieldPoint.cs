using UnityEngine;

public class StarfieldPoint
{
    private Vector2 initPos;
    private Vector2 offset;
    private Vector2 vel;
    private Vector4 properties; // x-pos, y-pos, radius, intensity
	private Vector3 colour;
    
    public float PositionX
    {
        get { return this.properties.x; }
    }
    
    public float PositionY
    {
        get { return this.properties.y; }
    }
    
    public float Radius
    {
        get { return this.properties.z; }
        set { this.properties.z = value; }
    }
    
    public float Intensity
    {
        get { return this.properties.w; }
        set { this.properties.w = value; }
    }

	public Vector3 Colour 
	{ 
		get { return this.colour; } 
		set { this.colour = value; }
	}
    
    public Vector4 Properties { get { return this.properties; } }


	public StarfieldPoint(Vector2 position, Vector2 velocity, float radius, float intensity, Vector3 colour)
    {
        this.vel = velocity;
        
        this.initPos = position;   
        
        this.Radius = radius;
        this.Intensity = intensity;

		this.Colour = colour;
	}
    
    public void UpdateBounce(float dt, Vector4 bounds)
    {
        var hitXMin = (this.PositionX - this.Radius) < bounds.x && this.vel.x < 0f;
        var hitXMax = (this.PositionX + this.Radius) > bounds.y && this.vel.x > 0f;

        if (hitXMin || hitXMax)
        {
            this.vel.x *= -1;
        }
    
        var hitZMin = (this.PositionY - this.Radius) < bounds.z && this.vel.y < 0f;
        var hitZMax = (this.PositionY + this.Radius) > bounds.w && this.vel.y > 0f;
    
        if (hitZMin || hitZMax)
        {
            this.vel.y *= -1;
        }
 
        this.UpdateOffset(this.offset += this.vel * dt);
    }
    
    public void UpdateOffset(Vector2 offset)
    {
        this.offset = offset;
        this.properties.x = this.initPos.x + offset.x;
        this.properties.y = this.initPos.y + offset.y;
    }
}
