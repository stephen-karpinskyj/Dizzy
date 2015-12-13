#pragma strict

var paths: iTweenPath[];
var dots: Transform[];

var traceSpeed: float = 10;
var traceDistace = 0.5f;
var tracePointInterval = 0.1f;
var tracer: GameObject;

var lastPointTraced: Vector3;

var waiting: boolean;
var prev: Transform;

function Start () {
    Broadcast.SendMessage("LevelStart");
}

function LevelStart () {
    iTween.Stop(gameObject);
    StopAllCoroutines();

    StartCoroutine(LevelStartCoroutine());
}

function LevelWin () {
    iTween.Stop(gameObject);
    StopAllCoroutines();

    StartCoroutine(LevelStartCoroutine());
}

function LevelStartCoroutine () {
    waiting = false;

    for (var path: iTweenPath in paths) {
        var points = iTweenPath.GetPath(path.pathName);
        var args = iTween.Hash("path", points, "speed", traceSpeed, "onupdate", "OnUpdate", "oncomplete", "OnComplete");

        transform.position = points[0];
        lastPointTraced = Vector3.zero;

        iTween.MoveTo(gameObject, args);
        waiting = true;

        while (waiting) {
            yield null;
        }
    }

    for (var dot: Transform in dots) {
        transform.position = dot.position;
        transform.rotation = dot.rotation;
        Instantiate(tracer, transform.position, transform.rotation);

        yield WaitForSeconds(tracePointInterval);
    }
}

function OnUpdate() {
    if (Vector3.Distance(transform.position, lastPointTraced) > traceDistace) {
        lastPointTraced = transform.position;

        var curr = Instantiate(tracer, transform.position, transform.rotation).transform;
        
        if (prev) {
            var dir = curr.position - prev.position;
            curr.eulerAngles = Vector3(0, 0, Mathf.Atan2(dir.x, -dir.y) * Mathf.Rad2Deg + 90);
        }

        prev = curr;
    }
}

function OnComplete() {
    waiting = false;
}
