#pragma strict

import System.Collections.Generic;
import UnityEngine.UI;

var message = "<b>{0}x</b>";

var text: Text;

var secondsUntilExpire: float = 0.5f;
var maxMultiplier: int = 10;

var blast: GameObject;
var delayBetweenBlasts: float = 0.05f;

static var multiplier: int = 0;
var lastTimeHit: float;

var positions: List.<Vector3> = new List.<Vector3>();

var source: AudioSource;

function Update() {
    if ((Time.time - lastTimeHit) >= secondsUntilExpire) {
        if (!Input.anyKey) {
            multiplier = 0;
            lastTimeHit = 0;
        }
    }

    if (multiplier == 0) {
        text.text = "";
    } else {
        text.text = ""; //String.Format(message, multiplier);
    }
}

function LevelStart() {
    multiplier = 0;
    lastTimeHit = 0;
    positions.Clear();
}

function LevelWin() {
    multiplier = 0;
    lastTimeHit = 0;
    positions.Clear();

    source.Play();
}

function BombHit(bomb: List.<Vector3>) {
    if (multiplier == (maxMultiplier - 1)) {
        //StartCoroutine(BlowAllBombs());
        //multiplier = 0;
    }

    positions.Add(bomb[0]);

    multiplier++;
    lastTimeHit = Time.time;
}

function BlowAllBombs() {
    var p: List.<Vector3> = new List.<Vector3>(positions);
    positions.Clear();

    while (p.Count > 0) {
        var index = Random.Range(0, p.Count);
        var pos = p[index];

        Instantiate(blast, pos, Quaternion.identity);
        yield WaitForSeconds(delayBetweenBlasts);

        p.Remove(pos);
    }
}
