using UnityEngine;
using System.Collections;

public class BombController : MonoBehaviour
{
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

    private bool isPulling = false;

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
        if (!RocketController.Instance || !this.isPulling)
        {
            return;
        }

        var dir = (Vector2)(RocketController.Instance.Position - this.transform.position).normalized; // Ignore z

        var curveValue = this.gravityCurve.Evaluate((Time.time - this.startPullTime) / this.gravityCurveDuration);
        var rocketMultipler = 1f + RocketController.Instance.SpeedPercentage * this.rocketSpeedInfluence;
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
        else if (other.tag == this.junkPullerTag && !this.isPulling)
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
        this.isPulling = true;

        var vel = (Vector2)(this.transform.position - RocketController.Instance.Position).normalized;
        RocketController.Instance.AddForce(vel * this.attractForceMagnitude);

        this.rotationAtAttraction = Vector2.down.SignedAngle(vel);
    }

    private void Explode()
    {
        if (this.hasExploded)
        {
            return;
        }

        this.hasExploded = true;

        this.source.clip = this.deathClips[Random.Range(0, this.deathClips.Length)];
        this.source.volume = this.deathVolume;
        this.source.pitch = 1f + this.deathPitchMultiplier * MultiplierController.Instance.CurrentMultiplier;
        this.source.Play();

        foreach (var r in this.renderers)
        {
            r.enabled = false;
        }

        CameraController.Instance.Shake(this.explosionShakeDuration, this.explosionShakeMagnitude);

        this.transform.eulerAngles = Vector3.forward * this.rotationAtAttraction;

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
        }

        Object.Destroy(this.gameObject);
    }
}
