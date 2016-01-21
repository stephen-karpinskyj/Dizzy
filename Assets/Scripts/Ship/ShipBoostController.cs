using System.Collections;
using UnityEngine;

public class ShipBoostController : MonoBehaviour
{
    [SerializeField]
    private float shakeDuration = 0f;

    [SerializeField]
    private float shakeMagnitude = 0f;

    [SerializeField]
    private float boostColliderDisableDelay = 4 / 60f;

    [SerializeField]
    private ShipController ship;

    [SerializeField]
    private ParticleSystem boostParticles;

    [SerializeField]
    private ParticleSystemRenderer boostParticleRenderer;

    [SerializeField]
    private Collider2D boostCollider;

    [SerializeField]
    private Color ccwColor;

    [SerializeField]
    private Color cwColor;

    [SerializeField]
    private Color ccwLightColor;

    [SerializeField]
    private Color cwLightColor;

    [SerializeField]
    private string materialColorPropertyName = "_TintColor";

    [SerializeField]
    private Light boostLight;

    private bool wasCCW = true;

    private void Update()
    {
        var isCCW = this.ship.IsCCW;
        if (isCCW != this.wasCCW)
        {
            this.boostParticleRenderer.material.SetColor(this.materialColorPropertyName, isCCW ? this.ccwColor : this.cwColor);
            this.boostLight.color = isCCW ? this.ccwLightColor : this.cwLightColor;
            this.wasCCW = isCCW;
        }

        if (this.ship.IsThrusting)
        {
            //this.StopCoroutine("DelayColliderDisable");

            if (!this.boostParticles.isPlaying)
            {
                this.boostParticles.Play();
                this.boostLight.enabled = true;
            }
            //this.boostCollider.enabled = true;
            CameraController.Shake(this.shakeDuration, this.shakeMagnitude);
        }
        else
        {
            if (this.boostParticles.isPlaying)
            {
                this.boostParticles.Stop();
                this.boostLight.enabled = false;
            }
            
            //this.StartCoroutine("DelayColliderDisable");
        }
    }

    private IEnumerator DelayColliderDisable()
    {
        yield return new WaitForSeconds(this.boostColliderDisableDelay);
        this.boostCollider.enabled = false; 
    }
}
