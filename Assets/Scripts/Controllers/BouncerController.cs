using System.Collections;
using UnityEngine;

public class BouncerController : MonoBehaviour
{
    [SerializeField]
    private Collider2D coll;

    [SerializeField]
    private Renderer rend;

    [SerializeField]
    private float minBounceSpeed = 2f;

    [SerializeField]
    private float bounceFrictionMultiplier = 0.8f;

    [SerializeField]
    private float flashDuration = 0.05f;

    [SerializeField]
    private Color flashColour = new Color(148/255f, 136/255f, 182/255f);

    [SerializeField]
    private float shakeDuration = 0.2f;

    [SerializeField]
    private float shakeMagnitude = 0.1f;

    [SerializeField]
    private AudioSource source;

    [SerializeField]
    private bool passThroughOnce = true;

    private Color defaultColour;

    private bool hasExited = false;

    private void Awake()
    {
        this.defaultColour = this.rend.sharedMaterial.color;

        this.hasExited = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (this.passThroughOnce && !this.hasExited)
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
        this.rend.sharedMaterial.color = flashColour;
        yield return new WaitForSeconds(this.flashDuration);
        this.rend.sharedMaterial.color = defaultColour;
    }

    private void LevelStart()
    {
        this.StopAllCoroutines();
        this.rend.sharedMaterial.color = defaultColour;

        this.hasExited = false;
    }

    private void LevelWin()
    {
        this.StopAllCoroutines();
        this.rend.sharedMaterial.color = defaultColour;

        this.hasExited = false;
    }
}
