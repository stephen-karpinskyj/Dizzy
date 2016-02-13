using UnityEngine;
using UnityEngine.UI;

public class BeaconPingUIController : MonoBehaviour
{
    [SerializeField]
    private GameObject parent;
    
    [SerializeField]
    private WorldToUI particleWorldToUi;
    
    [SerializeField]
    private WorldToUI textWorldToUi;
    
    [SerializeField]
    private ParticleSystem particles;
    
    [SerializeField]
    private Text text;
    
    [SerializeField]
    private RectTransform textRect;
    
    private Transform target;
    
    public void Initialise(Transform target)
    {
        this.particleWorldToUi.Target = target;
        this.textWorldToUi.Target = target;
        this.target = target;
    }
    
    public void Show(bool show)
    {
        this.parent.SetActive(show);
        this.particles.Play();
    }
    
    private void Update()
    {
        var ship = GameManager.Instance.Ship.Trans.position;
        var dist = Vector2.Distance(ship, this.target.position);
        
        if (this.particleWorldToUi.IsConstrained)
        {
            if (!this.text.enabled)
            {
                this.text.enabled = true;
            }
            
            this.text.text = FormattingUtility.DistanceToString(dist);
        }
        else
        {
            if (this.text.enabled)
            {
                this.text.enabled = false;
            }
        }
    }
}
