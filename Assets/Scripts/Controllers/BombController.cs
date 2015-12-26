using UnityEngine;

public class BombController : MonoBehaviour
{
    [SerializeField]
    private float explosionMagnitude = 26f;
    
    [SerializeField]
    private float otherSpeedDecayMultiplier = 0.85f;

    [SerializeField]
    private float angleDiffCorrectionTolerance = 20f;

    [SerializeField]
    private float correctionTorque = 10f;

    [SerializeField]
    private AnimationCurve correctionTorqueCurve;

    [SerializeField]
    private float explosionShakeMagnitude = 0.06f;

    [SerializeField]
    private float explosionShakeDuration = 0.2f;

    [SerializeField]
    private float destroyDelay = 0.2f;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private ParticleSystem explosionParticles;

    [SerializeField]
    private Vector2 explosionMultiplierRange = new Vector2(0.3f, 1f);

    [SerializeField]
    private AudioSource source;

    [SerializeField]
    private AudioClip spawnClip;

    [SerializeField]
    private AudioClip deathClip;

    [SerializeField]
    private float spawnVolume = 0.05f;

    [SerializeField]
    private Vector2 spawnPitchRange = new Vector2(0.95f, 1.1f);

    [SerializeField]
    private float deathPitchMultiplier = 0.02f;

    private bool hasExploded = false;

    public bool HasExploded
    {
        get { return this.hasExploded; }
    }

    private void Awake()
    {
        Debug.Assert(this.spriteRenderer, this);
        Debug.Assert(this.explosionParticles, this);
        Debug.Assert(this.source, this);
        Debug.Assert(this.spawnClip, this);
        Debug.Assert(this.deathClip, this);

        this.source.clip = this.spawnClip;
        this.source.volume = this.spawnVolume;
        this.source.pitch = Random.Range(this.spawnPitchRange.x, this.spawnPitchRange.y);
        this.source.Play();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (this.hasExploded)
        {
            return;
        }

        this.hasExploded = true;
        Object.Destroy(this.gameObject, this.destroyDelay);

        var rocket = Object.FindObjectOfType<RocketController>();

        var angleInRadians = transform.eulerAngles.z * Mathf.Deg2Rad;
        var aimDir = other.transform.right;
        var forceDir = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians)).normalized;
        var velDir = rocket.MainRigidbody.velocity.normalized;
        var diffAngle = aimDir.SignedAngle(forceDir);
        var absDiffAngle = Mathf.Abs(diffAngle);
        var diffDot = Vector2.Dot(velDir, forceDir);

        if (Input.anyKey && absDiffAngle < this.angleDiffCorrectionTolerance)
        {
            var curveValue = this.correctionTorqueCurve.Evaluate(absDiffAngle / this.angleDiffCorrectionTolerance);
            rocket.MainRigidbody.AddTorque(this.correctionTorque * Mathf.Sign(diffAngle) * curveValue, ForceMode2D.Impulse);
        }

        var explosionMultiplier = Mathf.Lerp(this.explosionMultiplierRange.x, this.explosionMultiplierRange.y, diffDot);

        rocket.MainRigidbody.velocity *= this.otherSpeedDecayMultiplier;
        rocket.MainRigidbody.AddForce(forceDir * this.explosionMagnitude * explosionMultiplier, ForceMode2D.Impulse);

        if (rocket.MainRigidbody.velocity.magnitude > rocket.MaxSpeed)
        {
            rocket.MainRigidbody.velocity = rocket.MainRigidbody.velocity.normalized * rocket.MaxSpeed;
        }

        this.source.clip = this.deathClip;
        this.source.volume = 1f;
        this.source.pitch = 1f + this.deathPitchMultiplier * MultiplierController.Instance.CurrentMultiplier;
        this.source.Play();

        this.spriteRenderer.enabled = false;

        CameraController.Instance.Shake(this.explosionShakeDuration, this.explosionShakeMagnitude * explosionMultiplier);

        this.explosionParticles.startLifetime *= explosionMultiplier;
        this.explosionParticles.Play();

        MultiplierController.Instance.Increment();
    }

    private void LevelStart()
    {
        Object.Destroy(this.gameObject);
    }

    private void LevelWin()
    {
        Object.Destroy(this.gameObject);
    }
}
