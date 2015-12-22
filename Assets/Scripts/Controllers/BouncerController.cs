using System.Collections;
using UnityEngine;

public class BouncerController : MonoBehaviour
{
    [SerializeField]
    private Collider2D coll;

    [SerializeField]
    private Renderer rend;

    [SerializeField]
    private float minBounceSpeed = 0.5f;

    [SerializeField]
    private float bounceFrictionMultiplier = 0.9f;

    [SerializeField]
    private float flashDuration = 0.05f;

    [SerializeField]
    private Color flashColour = Color.blue;

    [SerializeField]
    private float shakeDuration = 0.05f;

    [SerializeField]
    private float shakeMagnitude = 0.1f;

    [SerializeField]
    private AudioSource source;

    private Color defaultColour;

    private bool hasExited = false;

    private void Awake()
    {
        this.defaultColour = this.rend.material.color;

        this.hasExited = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!this.hasExited)
        {
            return;
        }

        if (!other.name.StartsWith("Rocket"))
        {
            return;
        }

        var bounceVelocity = other.attachedRigidbody.velocity;
        var mask = LayerMask.GetMask(LayerMask.LayerToName(this.gameObject.layer));
        var hit = Physics2D.Raycast(other.transform.position, bounceVelocity.normalized, 20f, mask);
        var angle = Mathf.Atan2(bounceVelocity.y, bounceVelocity.x) * Mathf.Rad2Deg;
        var normAngle = Mathf.Atan2(hit.normal.y, hit.normal.x) * Mathf.Rad2Deg;
        var bounceAngle = 2 * normAngle - 180 - angle;

        DebugLine.Draw(hit.point, hit.point + hit.normal * 0.4f, Color.red, 5f);

        var bounceSpeed = Mathf.Max(bounceVelocity.magnitude * this.bounceFrictionMultiplier, this.minBounceSpeed);
        bounceVelocity.x = Mathf.Cos(bounceAngle * Mathf.Deg2Rad) * bounceSpeed;
        bounceVelocity.y = Mathf.Sin(bounceAngle * Mathf.Deg2Rad) * bounceSpeed;

        other.attachedRigidbody.velocity = bounceVelocity;

        this.StopAllCoroutines();
        this.StartCoroutine(this.FlashCoroutine());

        CameraController.Instance.Shake(this.shakeDuration, this.shakeMagnitude);

        this.source.Play();
    }

    private void OnTriggerExit2D() 
    {
        this.hasExited = true;
    }

    private IEnumerator FlashCoroutine()
    {
        this.rend.material.color = flashColour;
        yield return new WaitForSeconds(this.flashDuration);
        this.rend.material.color = defaultColour;
    }

    private void LevelStart()
    {
        this.StopAllCoroutines();
        this.rend.material.color = defaultColour;

        this.hasExited = false;
    }

    private void LevelWin()
    {
        this.StopAllCoroutines();
        this.rend.material.color = defaultColour;

        this.hasExited = false;
    }
}
