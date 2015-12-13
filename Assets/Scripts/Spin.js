#pragma strict

var magnitude: Vector3 = Vector3(0, -1, 0);

function Update () {
    transform.localRotation = transform.localRotation * Quaternion.Euler(Time.smoothDeltaTime * magnitude);
}