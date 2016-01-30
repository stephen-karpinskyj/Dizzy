using System.Collections;
using UnityEngine;

public class ShipBoostController : MonoBehaviour
{
    [SerializeField]
    private float shakeDuration = 1 / 60f;

    [SerializeField]
    private float shakeMagnitude = 0f;

    [SerializeField]
    private float boostColliderDisableDelay = 4 / 60f;

    [SerializeField]
    private Collider2D smallBoostCollider;

    [SerializeField]
    private ShipController ship;

    [SerializeField]
    private ParticleSystem boostParticles;

    [SerializeField]
    private ParticleSystemRenderer boostParticleRenderer;

    [SerializeField]
    private Collider2D boostCollider;

    [SerializeField]
    private Color normalColor;

    [SerializeField]
    private Color overdriveColor;

    [SerializeField]
    private Color normalLightColor;

    [SerializeField]
    private Color overdriveLightColor;

    [SerializeField]
    private string materialColorPropertyName = "_TintColor";

    [SerializeField]
    private Light boostLight;

    private bool isOverdrive = true;
    
    private ParticleSystem.EmissionModule boostParticleEmission;
    private ParticleSystem.MinMaxCurve boostParticleRate;
    private float initBoostParticleRateMax;
    
    private void ChangeColour(bool isOverdrive)
    {
        if (this.isOverdrive == isOverdrive)
        {
            return;
        }
        
        this.boostParticleRenderer.material.SetColor(this.materialColorPropertyName, isOverdrive ? this.normalColor : this.overdriveColor);
        this.boostLight.color = isOverdrive ? this.normalLightColor : this.overdriveLightColor;
        
        this.isOverdrive = isOverdrive;
    }
    
    private void Awake()
    {
        this.boostParticleEmission = this.boostParticles.emission;
        this.boostParticleRate = this.boostParticleEmission.rate;
        this.initBoostParticleRateMax = this.boostParticleRate.constantMax;
    }
    
    private void Update()
    {
        this.ChangeColour(!this.ship.InOverdrive);

        if (this.ship.IsThrusting)
        {
            this.StopCoroutine("DelayColliderDisable");

            if (this.boostParticleRate.constantMax == 0f)
            {
                this.boostParticleRate.constantMax = this.initBoostParticleRateMax;
                this.boostParticleEmission.rate = this.boostParticleRate;
                this.boostLight.enabled = true;
            }
            this.smallBoostCollider.enabled = true;
            
            if (this.ship.InOverdrive)
            {
                CameraController.Shake(this.shakeDuration, this.shakeMagnitude);
            }
        }
        else
        {
            if (this.boostParticleRate.constantMax != 0f)
            {
                this.boostParticleRate.constantMax = 0f;
                this.boostParticleEmission.rate = this.boostParticleRate;
            }
            
            this.StartCoroutine("DelayColliderDisable");
        }
    }

    private IEnumerator DelayColliderDisable()
    {
        this.boostLight.enabled = false;
        
        yield return new WaitForSeconds(this.boostColliderDisableDelay);
        this.smallBoostCollider.enabled = false;
    }
}
