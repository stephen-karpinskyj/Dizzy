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
    private Collider2D boostCollider;

    private void Update()
    {    
        if (Input.anyKey)
        {
            this.StopCoroutine("DelayColliderDisable");

            this.boostParticles.Play();
            this.boostCollider.enabled = true;
            CameraController.Instance.Shake(this.shakeDuration, this.shakeMagnitude);
        }
        else
        {
            this.boostParticles.Stop();
            this.StartCoroutine("DelayColliderDisable");
        }
    }

    private IEnumerator DelayColliderDisable()
    {
        yield return new WaitForSeconds(this.boostColliderDisableDelay);
        this.boostCollider.enabled = false; 
    }
}
