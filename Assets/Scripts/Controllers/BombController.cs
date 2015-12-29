using UnityEngine;
using System.Collections;
using System.Linq;

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
    private Renderer[] renderers;

    [SerializeField]
    private ParticleSystem explosionParticles;

    [SerializeField]
    private Vector2 explosionMultiplierRange = new Vector2(0.3f, 1f);

    [SerializeField]
    private AudioSource source;

    [SerializeField]
    private AudioClip spawnClip;

    [SerializeField]
    private AudioClip[] deathClips;

    [SerializeField]
    private float spawnVolume = 0.05f;

    [SerializeField]
    private Vector2 spawnPitchRange = new Vector2(0.95f, 1.1f);

    [SerializeField]
    private float deathVolume = 0.1f;

    [SerializeField]
    private float deathPitchMultiplier = 0.02f;

    [SerializeField]
    private string junkPullerTag = "JunkPuller";

    [SerializeField]
    private string junkDestroyerTag = "JunkDestroyer";

    [SerializeField]
    private float attractForceMagnitude = 20f;

    [SerializeField]
    private AnimationCurve gravityCurve;

    [SerializeField]
    private float gravityCurveDuration = 0.2f;

    [SerializeField]
    private float rocketSpeedInfluence = 3f;

    [SerializeField]
    private Vector2 speedMultiplierRange = new Vector2(20f, 80f);

    private float startPullTime;

    private bool isPulled = false;

    private bool hasExploded = false;

    private float rotationAtAttraction;

    public bool HasExploded
    {
        get { return this.hasExploded; }
    }

    private void Awake()
    {
        Debug.Assert(this.explosionParticles, this);
        Debug.Assert(this.source, this);
        Debug.Assert(this.spawnClip, this);

        this.source.clip = this.spawnClip;
        this.source.volume = this.spawnVolume;
        this.source.pitch = Random.Range(this.spawnPitchRange.x, this.spawnPitchRange.y);
        this.source.Play();

        var index = Random.Range(0, this.renderers.Length);
        for (int i = 0; i < this.renderers.Length; i++)
        {
            this.renderers[i].enabled = index == i;
        }
    }

    private void FixedUpdate()
    {
        if (!RocketController.Instance || !this.isPulled)
        {
            return;
        }

        var dir = (Vector2)(RocketController.Instance.transform.position - this.transform.position).normalized; // Ignore z

        var curveValue = this.gravityCurve.Evaluate((Time.time - this.startPullTime) / this.gravityCurveDuration);
        var rocketMultipler = 1f + (RocketController.Instance.MainRigidbody.velocity.magnitude / RocketController.Instance.MaxSpeed) * this.rocketSpeedInfluence;
        var multiplier = Mathf.Lerp(this.speedMultiplierRange.x, this.speedMultiplierRange.y, curveValue);
        var speed = multiplier * rocketMultipler * Time.fixedDeltaTime;

        var velocity = (Vector3)dir * speed;

        var pos = this.transform.position;
        pos += velocity;
        this.transform.position = pos;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (this.hasExploded)
        {
            return;
        }

        if (other.tag == this.junkDestroyerTag && !this.hasExploded)
        {
            this.Explode();
        }
        else if (other.tag == this.junkPullerTag && !this.isPulled)
        {
            this.StartPulling();
        }
    }

    private void LevelStart()
    {
        Object.Destroy(this.gameObject);
    }

    private void LevelWin()
    {
        Object.Destroy(this.gameObject);
    }

    private void StartPulling()
    {
        this.startPullTime = Time.time;
        this.isPulled = true;

        var rocket = RocketController.Instance;
        var vel = (Vector2)(this.transform.position - rocket.transform.position).normalized;
        rocket.MainRigidbody.AddForce(vel * this.attractForceMagnitude, ForceMode2D.Impulse);

        if (rocket.MainRigidbody.velocity.magnitude > rocket.MaxSpeed)
        {
            rocket.MainRigidbody.velocity = rocket.MainRigidbody.velocity.normalized * rocket.MaxSpeed;
        }

        this.rotationAtAttraction = Vector2.down.SignedAngle(vel);
    }

    private void Explode()
    {
        if (this.hasExploded)
        {
            return;
        }

        var rocket = RocketController.Instance;

        if (!rocket)
        {
            return;
        }

        this.hasExploded = true;

        /*
        var angleInRadians = transform.eulerAngles.z * Mathf.Deg2Rad;
        var aimDir = rocket.transform.right;
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
         */

        var explosionMultiplier = 1f; //Mathf.Lerp(this.explosionMultiplierRange.x, this.explosionMultiplierRange.y, diffDot);

        /*rocket.MainRigidbody.velocity *= this.otherSpeedDecayMultiplier;
        rocket.MainRigidbody.AddForce(forceDir * this.explosionMagnitude * explosionMultiplier, ForceMode2D.Impulse);

        if (rocket.MainRigidbody.velocity.magnitude > rocket.MaxSpeed)
        {
            rocket.MainRigidbody.velocity = rocket.MainRigidbody.velocity.normalized * rocket.MaxSpeed;
        }*/

        this.source.clip = this.deathClips[Random.Range(0, this.deathClips.Length)];
        this.source.volume = this.deathVolume;
        this.source.pitch = 1f + this.deathPitchMultiplier * MultiplierController.Instance.CurrentMultiplier;
        this.source.Play();

        foreach (var r in this.renderers)
        {
            r.enabled = false;
        }

        CameraController.Instance.Shake(this.explosionShakeDuration, this.explosionShakeMagnitude * explosionMultiplier);

        this.transform.eulerAngles = Vector3.forward * this.rotationAtAttraction;

        this.explosionParticles.startLifetime *= explosionMultiplier;
        this.explosionParticles.Play();

        MultiplierController.Instance.Increment();

        this.StartCoroutine(this.DestroyCoroutine());
    }

    private IEnumerator DestroyCoroutine()
    {
        var startTime = Time.time;

        while ((Time.time - startTime) < this.destroyDelay)
        {
            yield return null;

            var bombs = Object.FindObjectsOfType<BombController>();

            if (bombs.Length <= 1 || bombs.All(b => b.hasExploded))
            {
                break;
            }
        }

        Object.Destroy(this.gameObject);
    }
}
