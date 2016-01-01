using System.Collections;
using UnityEngine;

public class RocketBoostController : MonoBehaviour
{
    [SerializeField]
    private float shakeDuration = 0f;

    [SerializeField]
    private float shakeMagnitude = 0f;

    [SerializeField]
    private float boostColliderDisableDelay = 4 / 60f;

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
    private Light light;

    private bool wasCCW = true;

    private void Update()
    {
        var isCCW = RocketController.Instance.IsCCW;
        if (isCCW != this.wasCCW)
        {
            this.boostParticleRenderer.material.SetColor(this.materialColorPropertyName, isCCW ? this.ccwColor : this.cwColor);
            this.light.color = isCCW ? this.ccwLightColor : this.cwLightColor;
            this.wasCCW = isCCW;
        }

        if (RocketController.Instance.IsThrusting)
        {
            //this.StopCoroutine("DelayColliderDisable");

            this.boostParticles.Play();
            this.light.enabled = true;
            //this.boostCollider.enabled = true;
            CameraController.Instance.Shake(this.shakeDuration, this.shakeMagnitude);
        }
        else
        {
            this.boostParticles.Stop();
            this.light.enabled = false;
            //this.StartCoroutine("DelayColliderDisable");
        }
    }

    private IEnumerator DelayColliderDisable()
    {
        yield return new WaitForSeconds(this.boostColliderDisableDelay);
        this.boostCollider.enabled = false; 
    }
}
