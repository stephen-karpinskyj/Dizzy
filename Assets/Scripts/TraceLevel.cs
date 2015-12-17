using System.Collections;
using UnityEngine;

public class TraceLevel : MonoBehaviour
{
    [SerializeField]
    private float traceSpeed = 15;

    [SerializeField]
    private float traceDistace = 0.8f;

    [SerializeField]
    private float tracePointInterval = 0.1f;

    [SerializeField]
    private GameObject tracerPrefab;

    private iTweenPath[] paths;
    private Dot[] dots;

    private Vector3 lastPointTraced;
    private bool isWaiting;
    private Transform prev;

    private void Start()
    {
        Debug.Assert(this.tracerPrefab, this);

        this.paths = this.transform.parent.GetComponentsInChildren<iTweenPath>();
        this.dots = this.transform.parent.GetComponentsInChildren<Dot>();

        Broadcast.SendMessage("LevelStart");
    }

    private void LevelStart()
    {
        iTween.Stop(this.gameObject);
        this.StopAllCoroutines();

        this.StartCoroutine(this.LevelStartCoroutine());
    }

    private void LevelWin()
    {
        iTween.Stop(this.gameObject);
        this.StopAllCoroutines();

        this.StartCoroutine(this.LevelStartCoroutine());
    }

    private IEnumerator LevelStartCoroutine()
    { 
        yield return new WaitForSeconds(0.05f);

        this.isWaiting = false;

        foreach (var path in this.paths)
        {
            var points = iTweenPath.GetPath(path.pathName);
            var args = iTween.Hash("path", points, "speed", traceSpeed, "onupdate", "OnPathTraceUpdate", "oncomplete", "OnPathTraceComplete");

            transform.position = points[0];
            lastPointTraced = Vector3.zero;

            iTween.MoveTo(gameObject, args);
            isWaiting = true;

            while (isWaiting)
            {
                yield return null;
            }
        }

        foreach (var dot in this.dots)
        {
            this.transform.position = dot.transform.position;
            this.transform.rotation = dot.transform.rotation;
            Object.Instantiate(this.tracerPrefab, this.transform.position, this.transform.rotation);

            yield return new WaitForSeconds(this.tracePointInterval);
        }
    }

    private void OnPathTraceUpdate()
    {
        if (Vector3.Distance(this.transform.position, this.lastPointTraced) > this.traceDistace)
        {
            this.lastPointTraced = this.transform.position;

            var obj = Object.Instantiate(this.tracerPrefab, this.transform.position, this.transform.rotation) as GameObject;
            var curr = obj.transform;

            if (this.prev)
            {
                var dir = curr.position - this.prev.position;
                curr.eulerAngles = new Vector3(0f, 0f, Mathf.Atan2(dir.x, -dir.y) * Mathf.Rad2Deg + 90f);
            }

            this.prev = curr;
        }
    }

    private void OnPathTraceComplete()
    {
        this.isWaiting = false;
    }
}
