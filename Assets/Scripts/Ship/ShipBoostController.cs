using System.Collections;
using UnityEngine;

public class ShipBoostController : MonoBehaviour
{
    /*[SerializeField]
    private float shakeDuration = 0.03f;

    [SerializeField]
    private float shakeMagnitude = 1f;*/

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
    
    [SerializeField]
    private Vector2 boostLightIntensityRange = new Vector2(0f, 3f);
    
    [SerializeField]
    private float boostLightIntensityChangeSpeed = 20f;

    [SerializeField]
    private AudioSource source;

    [SerializeField]
    private float pitchIncreaseMultiplier = 0.4f;

    private bool isBoosting = false;
    private bool isOverdrive = true;

    private float targetBoostLightIntensity = 0f;
    private float currBoostLightIntensity = 0f;
    
    private ParticleSystem.EmissionModule boostParticleEmission;
    private ParticleSystem.MinMaxCurve boostParticleRate;
    private float initBoostParticleRateMax;
    
    private void Awake()
    {
        Debug.Assert(this.source != null, this);

        this.boostParticleEmission = this.boostParticles.emission;
        this.boostParticleRate = this.boostParticleEmission.rateOverTime;
        this.initBoostParticleRateMax = this.boostParticleRate.constantMax;
        
        this.StopBoosting();
    }
    
    private void Update()
    {
        this.ChangeColour(!this.ship.InOverdrive);

        if (GameManager.Instance.IsRunning)
        {
            if (this.ship.IsThrusting)
            {
                if (!this.isBoosting)
                {
                    this.StartBoosting();
                }
                
                this.source.pitch = 1f + this.ship.TimeThrusting * this.pitchIncreaseMultiplier;
                
                /*if (this.ship.InOverdrive)
                {
                    CameraController.Shake(this.shakeDuration, this.shakeMagnitude);
                }*/
            }
            else
            {
                if (this.isBoosting)
                {
                    this.StopBoosting();
                }
            }
        }
        else
        {
            if (this.isBoosting)
            {
                this.StopBoosting();
            }
        }
        
        if (!Mathf.Approximately(this.currBoostLightIntensity, this.targetBoostLightIntensity))
        {
            var dir = this.currBoostLightIntensity > this.targetBoostLightIntensity ? -1 : 1;
            this.currBoostLightIntensity += dir * this.boostLightIntensityChangeSpeed * Time.smoothDeltaTime;
            this.currBoostLightIntensity = Mathf.Clamp(this.currBoostLightIntensity, this.boostLightIntensityRange.x, this.boostLightIntensityRange.y);
            this.boostLight.intensity = this.currBoostLightIntensity;
        }
    }
    
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

    private void StartBoosting()
    {        
        this.isBoosting = true;
        
        this.boostParticleRate.constantMax = this.initBoostParticleRateMax;
        this.boostParticleEmission.rateOverTime = this.boostParticleRate;
        
        AudioManager.Instance.Play(this.source);
        this.targetBoostLightIntensity = this.boostLightIntensityRange.y;
        
        this.smallBoostCollider.enabled = true;
        
        this.StopCoroutine("DelayColliderDisable");
    }

    private void StopBoosting()
    {
        this.isBoosting = false;
        
        this.boostParticleRate.constantMax = 0f;
        this.boostParticleEmission.rateOverTime = this.boostParticleRate;

        AudioManager.Instance.Stop(this.source);
        this.targetBoostLightIntensity = this.boostLightIntensityRange.x;
    }

    private IEnumerator DelayColliderDisable()
    {        
        yield return new WaitForSeconds(this.boostColliderDisableDelay);
        this.smallBoostCollider.enabled = false;
                
        this.StartCoroutine("DelayColliderDisable");
    }
}
