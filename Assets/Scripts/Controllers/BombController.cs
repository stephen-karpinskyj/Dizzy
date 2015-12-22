using UnityEngine;

public class BombController : MonoBehaviour
{
    [SerializeField]
    private float explosionMagnitude = 26f;

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

    private static int CurrentCount = 0;

    private void Awake()
    {
        Debug.Assert(this.spriteRenderer, this);
        Debug.Assert(this.explosionParticles, this);
        Debug.Assert(this.source, this);
        Debug.Assert(this.spawnClip, this);
        Debug.Assert(this.deathClip, this);

        CurrentCount++;

        this.source.clip = this.spawnClip;
        this.source.volume = this.spawnVolume;
        this.source.pitch = Random.Range(this.spawnPitchRange.x, this.spawnPitchRange.y);
        this.source.Play();
    }

    private void OnDestroy()
    {
        CurrentCount--;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (this.hasExploded)
        {
            return;
        }

        this.hasExploded = true;
        Object.Destroy(this.gameObject, this.destroyDelay);

        var angleInRadians = transform.eulerAngles.z * Mathf.Deg2Rad;
        var forceDirection = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians)).normalized;

        var travelling = other.attachedRigidbody.velocity.normalized;
        var explosionMultiplier = Mathf.Lerp(this.explosionMultiplierRange.x, this.explosionMultiplierRange.y, Mathf.Max(0f, Vector2.Dot(travelling, forceDirection)));

        this.source.clip = this.deathClip;
        this.source.volume = 1f;
        this.source.pitch = 1f + this.deathPitchMultiplier * MultiplierController.Instance.CurrentMultiplier;
        this.source.Play();

        this.spriteRenderer.enabled = false;

        CameraController.Instance.Shake(this.explosionShakeDuration, this.explosionShakeMagnitude * explosionMultiplier);

        other.attachedRigidbody.AddForce(forceDirection * this.explosionMagnitude * explosionMultiplier, ForceMode2D.Impulse);

        this.explosionParticles.startLifetime *= explosionMultiplier;
        this.explosionParticles.Play();

        if (CurrentCount == 1)
        {
            Broadcast.SendMessage("LevelWin");
        }
        else
        {
            MultiplierController.Instance.Increment();
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
}
