#pragma strict

var target: GameObject;

function OnBecameInvisible () {
    Destroy(target);
}