#pragma strict

import UnityEngine.UI;

var message = "<color={0}>Tap to thrust</color>";
var text: Text;

var onColor: String = "#000000ff";
var offColor: String = "#ff00ffff";

function Update () {
    if (Time.time % 1f > 0.5f) {
        text.text = String.Format(message, offColor);
    } else {
        text.text = String.Format(message, onColor);
    }
}