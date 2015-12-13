#pragma strict

var boostForce = Vector2(250, 0);

var idleForce = Vector2(20, 0);
var torque = 2;

var maxAngularVelocity = 3;

var angularVelocityDecayOnPress = 0.7f;
var velocityDecayOnPress = 0.7f;

var rigidBody: Rigidbody2D;

var inWinMode: boolean = false;

var source: AudioSource;

static var Count: int = 0;

var timeHeld: float = 0;

function Start () {
    Count = Count + 1;

    if (Count == 1) {
        Broadcast.SendMessage("TimeStart");
    }
}

function OnDestroy () {
    Count = Count - 1;

    if (Count == 0 && !inWinMode) {
        Broadcast.SendMessage("LevelStart");
    }
}

function Update () {
    if (Input.anyKey) {
        timeHeld += Time.deltaTime;

        source.pitch = 1f + timeHeld * 0.3f;

        if (Input.anyKeyDown) {
            rigidBody.velocity *= velocityDecayOnPress;

            source.Play();
        }
        
        rigidBody.AddRelativeForce(boostForce);
        rigidBody.angularVelocity *= angularVelocityDecayOnPress;
    } else {
        rigidBody.AddRelativeForce(idleForce);
        rigidBody.AddTorque(torque);

        if (rigidBody.angularVelocity > maxAngularVelocity) {
            rigidBody.angularVelocity = maxAngularVelocity;
        }

        timeHeld = 0;

        source.Pause();
    }
}

function LevelWin() {
    inWinMode = true;
    Destroy(gameObject);
}

function LevelStart() {
    inWinMode = false;
    Destroy(gameObject);
}
