using UnityEngine;

public class WorldToUI : MonoBehaviour
{
    [SerializeField]
    private RectTransform rect;
    
    [SerializeField]
    private RectTransform canvasRect;
    
    [SerializeField]
    private bool raycast = true;
    
    public Transform Target { get; set; }
    
    public bool IsConstrained { get; private set; }
    
    private void OnEnable()
    {
        this.Update();
    }
    
	private void Update()
    {
        if (!this.Target)
        {
            return;
        }
        
        // Place it on canvas
        {
            var screenPos = UICamera.Instance.Camera.WorldToViewportPoint(this.Target.position);
            
            this.rect.anchoredPosition = new Vector2(
                (screenPos.x * this.canvasRect.sizeDelta.x) - (this.canvasRect.sizeDelta.x * 0.5f),
                (screenPos.y * this.canvasRect.sizeDelta.y) - (this.canvasRect.sizeDelta.y * 0.5f));
        }
        
        // Keep within screen bounds
        {
            var localPos = (Vector2)this.rect.localPosition;
            var aabb = new Bounds(Vector2.zero, this.canvasRect.rect.size - this.rect.rect.size);
                
            if (raycast)
            {                
                if (aabb.Contains(localPos))
                {
                    this.IsConstrained = false;
                }
                else
                {
                    float dist;
                    var origin = Vector2.zero;
                    var ray = new Ray(origin, (origin - localPos).normalized);
                    aabb.IntersectRay(ray, out dist);
                    localPos = ray.GetPoint(dist);
                    this.IsConstrained = true;
                }
            }
            else
            {
                var closestPos = (Vector2)aabb.ClosestPoint(localPos);
               
                if (localPos == closestPos)
                {
                    this.IsConstrained = false;
                }
                else
                {
                    localPos = closestPos;
                    this.IsConstrained = true;
                }
            }
            
            this.rect.localPosition = localPos;
        }
	}
}
