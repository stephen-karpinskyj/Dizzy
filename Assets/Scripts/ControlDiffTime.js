#pragma strict

import UnityEngine.UI;

var message = "<color={0}><b>Last:</b> {1:+0.000;-0.000;-0.000}s</color>";
var text: Text;

var defaultDiffColor: String = "#000000ff";
var negativeDiffColor: String = "#ff00ffff";

var currentColor: String;

var source: AudioSource;

var isChangingColor: boolean = false;

function Start () {
    currentColor = defaultDiffColor;
}

function Update () {
    var diffTime = PlayerPrefs.GetFloat("Diff");
    text.text = String.Format(message, currentColor, diffTime);
}

function PrevScoreChange () {
    if (PlayerPrefs.GetFloat("Diff") < 0f) {
        currentColor = negativeDiffColor;
        source.Play();
    } else {
        currentColor = defaultDiffColor;
    }

    if (isChangingColor) {
        return;
    }
}