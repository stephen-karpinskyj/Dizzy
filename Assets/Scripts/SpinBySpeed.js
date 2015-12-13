#pragma strict

var magnitude: Vector3 = Vector3(0, -1, 0);

var prevPos: Vector3;

function Start () {
    prevPos = transform.position;
}

function Update () {
    var dist = Vector3.Distance(transform.position, prevPos);

    transform.localRotation = transform.localRotation * Quaternion.Euler(Time.smoothDeltaTime * dist * magnitude);

    prevPos = transform.position;
}