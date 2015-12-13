#pragma strict

var explosionMagnitude: float = 10;
var explosionShakeMagnitude = 0.1f;
var explosionParticles: ParticleSystem;

var momentumDecayMultiplier: float = 0.8f;

var hasExploded: boolean = false;

var source: AudioSource;
var spawnClips: List.<AudioClip>;
var deathClips: List.<AudioClip>;

static var Count: int = 0;

function Start () {
    Count = Count + 1;

    source.clip = spawnClips[Random.Range(0, spawnClips.Count)];
    source.volume = 0.05f;
    source.pitch = Random.Range(0.95f, 1.1f);
    source.Play();
}

function OnDestroy () {
    Count = Count - 1;
}

function OnTriggerEnter2D (other: Collider2D) {
    if (hasExploded) {
        return;
    }

    hasExploded = true;

    source.clip = deathClips[Random.Range(0, deathClips.Count)];
    source.volume = 1f;
    source.pitch = 1f + 0.02f * ControlMultiplier.multiplier;
    source.Play();

    GetComponent(SpriteRenderer).enabled = false;

    ShakeCamera.Shake(0.2f, explosionShakeMagnitude);
    Destroy(gameObject, 0.2f);

    var angleInRadians = transform.eulerAngles.z * Mathf.Deg2Rad;
    var forceDirection = Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians)).normalized;
    other.attachedRigidbody.velocity *= momentumDecayMultiplier;
    other.attachedRigidbody.AddForce(forceDirection * explosionMagnitude, ForceMode2D.Impulse);

    explosionParticles.Play();

    if (Count == 1) {
        Broadcast.SendMessage("LevelWin");
    } else {
        var param = new List.<Vector3>();
        param.Add(transform.position);
        param.Add(transform.eulerAngles);

        Broadcast.SendMessage("BombHit", param);
    }
}
function LevelStart () {
    Destroy(gameObject);
}

function LevelWin () {
    Destroy(gameObject);
}
