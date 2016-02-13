using UnityEngine;

public class WorldToUI : MonoBehaviour
{
    [SerializeField]
    private RectTransform rect;
    
    [SerializeField]
    private RectTransform canvasRect;
    
    public Transform Target { get; set; }
    
    public bool IsConstrained { get; private set; }
    
	private void Update()
    {
        if (!this.Target)
        {
            return;
        }
        
        // Place it on canvas
        {
            Vector2 screenPos = UICamera.Instance.Camera.WorldToViewportPoint(this.Target.position);
            
            this.rect.anchoredPosition = new Vector2(
                (screenPos.x * this.canvasRect.sizeDelta.x) - (this.canvasRect.sizeDelta.x * 0.5f),
                (screenPos.y * this.canvasRect.sizeDelta.y) - (this.canvasRect.sizeDelta.y * 0.5f));
        }
        
        // Keep within screen bounds
        {
            var localPos = this.rect.localPosition;
            
            var canvasSize = this.canvasRect.rect.size;
            var canvasHalfSize = canvasSize / 2f;
            
            var halfSize = this.rect.rect.size / 2f;
            
            var horz = new Vector2(-canvasHalfSize.x + halfSize.x, canvasHalfSize.x - halfSize.x);
            var vert = new Vector2(-canvasHalfSize.y + halfSize.y, canvasHalfSize.y - halfSize.y);
            
            this.IsConstrained = false;
            
            if (localPos.x < horz.x)
            {
                localPos.x = horz.x;
                this.IsConstrained = true;
            }
            else if (localPos.x > horz.y)
            {
                localPos.x = horz.y;
                this.IsConstrained = true;
            }
            
            if (localPos.y < vert.x)
            {
                localPos.y = vert.x;
                this.IsConstrained = true;
            }
            else if (localPos.y > vert.y)
            {
                localPos.y = vert.y;
                this.IsConstrained = true;
            }
            
            this.rect.localPosition = localPos;
        }
	}
}
