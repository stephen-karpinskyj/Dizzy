#pragma strict

var shakeDuration: float = 0.1f;
var shakeMagnitude: float = 0.02f;

function Update () {
    if (Input.anyKey) {
        StopCoroutine("DelayColliderDisable");

        GetComponent(ParticleSystem).emission.enabled = true;
        GetComponent(Collider2D).enabled = true;
        ShakeCamera.Shake(shakeDuration, shakeMagnitude);
    } else {
        GetComponent(ParticleSystem).emission.enabled = false;
        StartCoroutine("DelayColliderDisable");
    }
}

function DelayColliderDisable() {
    yield WaitForSeconds(4 / 60f);
    GetComponent(Collider2D).enabled = false; 
}