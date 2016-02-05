using UnityEngine;

public class StarfieldPoint
{
    private Vector2 pos;
    private Vector2 vel;
    private Vector2 properties;
    
    public float Radius
    {
        get { return this.properties.x; }
        set { this.properties.x = value; }
    }
    
    public float Intensity
    {
        get { return this.properties.y; }
        set { this.properties.y = value; }
    }
    
    public Vector2 Position { get { return this.pos; } }
    public Vector2 Properties { get { return this.properties; } }

	public StarfieldPoint(Vector2 position, Vector2 velocity, float radius, float intensity)
    {
        this.pos = position;    
        this.vel = velocity;
        
        this.Radius = radius;
        this.Intensity = intensity;
	}
    
    public void Update(float dt, Vector4 bounds)
    {
        var hitXMin = (this.pos.x - this.Radius) < bounds.x && this.vel.x < 0f;
        var hitXMax = (this.pos.x + this.Radius) > bounds.y && this.vel.x > 0f;

        if (hitXMin || hitXMax)
        {
            this.vel.x *= -1;
        }
    
        var hitZMin = (this.pos.y - this.Radius) < bounds.z && this.vel.y < 0f;
        var hitZMax = (this.pos.y + this.Radius) > bounds.w && this.vel.y > 0f;
    
        if (hitZMin || hitZMax)
        {
            this.vel.y *= -1;
        }
 
        this.pos.x += this.vel.x * dt;
        this.pos.y += this.vel.y * dt;
    }
}
