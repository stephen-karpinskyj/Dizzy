#pragma strict

import UnityEngine.UI;

var message = "<b>Time:</b> <color={0}>{1:f2}s</color>";
var text: Text;
var defaultTargetTime: float = 90;
var panicStartTime: float = 10;

var countdownColor: String = "#000000ff";
var panicColor: String = "#ff0000ff";
var countupColor: String = "#ff0000ff";

var newHighScoreColor: String = "#ff00ffff";

var currentColor: String;

var isChangingColor: boolean = false;

var startTime: float;
var isRunning: boolean = false;

var source: AudioSource;
var isCountingDownAudio: boolean = false;

function LevelStart () {
    currentColor = countdownColor;
    isRunning = false;

    StopCoroutine("CountdownSound");
    isCountingDownAudio = false;
}

function TimeStart () {
    startTime = Time.time;

    if (!PlayerPrefs.HasKey("Time")) {
        PlayerPrefs.SetFloat("Time", defaultTargetTime);
    }

    isRunning = true;
}

function LevelWin () {
    var timePassed: float = Time.time - startTime;
    var bestTime: float = PlayerPrefs.GetFloat("Time", defaultTargetTime);
    var timeDiff: float = timePassed - bestTime;

    if (timePassed < bestTime) {
        PlayerPrefs.SetFloat("Time", timePassed);
        HighScoreChange();
    }

    PlayerPrefs.SetFloat("Diff", timeDiff);
    Broadcast.SendMessage("PrevScoreChange");
    
    currentColor = countdownColor;
    isRunning = false;

    StopCoroutine("CountdownSound");
    isCountingDownAudio = false;
}

function Update () {
    var numLeft = GameObject.FindObjectsOfType(ControlBomb).Length;

    if (isRunning) {
        var timeLeftFloat = PlayerPrefs.GetFloat("Time", defaultTargetTime) - (Time.time - startTime);
        var timeLeft = Mathf.Floor(timeLeftFloat);

        if (timeLeft < 0) {
            text.text = String.Format(message, countupColor, -timeLeft, numLeft);
        } else if (timeLeft <= panicStartTime) {
            if (!isCountingDownAudio) {
                StartCoroutine("CountdownSound");
                isCountingDownAudio = true;
            }

            if (timeLeftFloat % 1f > 0.5f) {
                text.text = String.Format(message, panicColor, timeLeftFloat.ToString("f1"), numLeft);
            } else {
                text.text = String.Format(message, countdownColor, timeLeftFloat.ToString("f1"), numLeft);
            }
        } else {
            text.text = String.Format(message, countdownColor, timeLeft, numLeft);
        }
    } else {
        var bestTime: float = PlayerPrefs.GetFloat("Time", defaultTargetTime);
        text.text = String.Format(message, currentColor, bestTime.ToString("f3"), numLeft);
    }
}

function CountdownSound() {
    for (var i: int = 0; i <= 6; i++) {
        source.Play();
        yield WaitForSeconds(1);
    }
}

function HighScoreChange () {
    if (isChangingColor) {
        return;
    }

    isChangingColor = true;

    for (var i = 0; i < 3; i++) {
        currentColor = countdownColor;
        yield WaitForSeconds(0.25f);

        currentColor = newHighScoreColor;
        yield WaitForSeconds(0.25f);
    }

    currentColor = countdownColor;
    isChangingColor = false;
}