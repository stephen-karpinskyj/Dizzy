#pragma strict

var projectile: GameObject;
var emitter: Transform;

var initialForce: Vector2 = Vector2(100, 0);

var shakeDuration: float = 0.1f;
var shakeMagnitude: float = 0.01f;

var timeToWaitOnWin: float = 0.5f;

var needToRelease: boolean = false;
var readyToGo: boolean = true;

var source: AudioSource;

var wasAnyKey: boolean = false;

function Update () {
    if (needToRelease) {
        if (wasAnyKey && !Input.anyKey) {
            needToRelease = false;
        } else {
            wasAnyKey = Input.anyKey;
            return;
        }
    }

    wasAnyKey = Input.anyKey;

    if (!readyToGo) {
        return;
    }

    if (Input.anyKey) {
        if (ControlRocket.Count == 0) {
            ShakeCamera.Shake(shakeDuration, shakeMagnitude);
            var rocket = Instantiate(projectile, emitter.position, emitter.rotation);
            rocket.GetComponent(Rigidbody2D).AddForce(initialForce, ForceMode2D.Impulse);

            source.Play();
        }
    }
}

function LevelWin() {
    StartCoroutine(BlockLaunch());

    if (Input.anyKey) {
        needToRelease = true;
    }
}

function BlockLaunch() {
    readyToGo = false;
    yield WaitForSeconds(timeToWaitOnWin);
    readyToGo = true;
}
