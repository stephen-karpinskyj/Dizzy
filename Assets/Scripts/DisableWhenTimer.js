#pragma strict

var mb : MonoBehaviour;
var enableOnWin: boolean = false;

function LevelWin () {
    mb.enabled = true;
}

function LevelStart () {
    if (!enableOnWin) {
        mb.enabled = true;
    } else {
        mb.enabled = false;
    }
}

function TimeStart () {
    mb.enabled = false;
}
