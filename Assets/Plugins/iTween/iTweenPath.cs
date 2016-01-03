//by Bob Berkebile : Pixelplacement : http://www.pixelplacement.com

using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class iTweenPath : MonoBehaviour
{
	public string pathName ="";
	public Color pathColor = Color.cyan;
	public List<Vector3> nodes = new List<Vector3>(){Vector3.zero, Vector3.zero};
	public int nodeCount;
	public static Dictionary<string, iTweenPath> paths = new Dictionary<string, iTweenPath>();
	public bool initialized = false;
	public string initialName = "";
    public Transform traceResultParent;
	
	void OnEnable(){
		paths[pathName.ToLower()] = this;
	}

    #if UNITY_EDITOR
	void OnDrawGizmos(){
		if(enabled) { // dkoontz
			if(nodes.Count > 0){
				iTween.DrawPath(nodes.ToArray(), pathColor);
			}
		} // dkoontz
    }
    #endif

    public float traceSpeed = 10f;

    public float traceDistance = 0.52f;

    private Vector3 lastPointTraced;

    private void OnPathTraceStart()
    {
        for (int i = 0; i < this.traceResultParent.childCount; i++)
        {
            Object.DestroyImmediate(this.traceResultParent.GetChild(i).gameObject);
        }

        this.transform.position = this.nodes[0];
        this.lastPointTraced = Vector3.zero;
    }

    private void OnPathTraceUpdate()
    {
        if (Vector3.Distance(this.transform.position, this.lastPointTraced) > this.traceDistance)
        {
            this.lastPointTraced = this.transform.position;

            var go = new GameObject("Dot");
            go.AddComponent<Dot>();
            go.transform.position = this.transform.position;
            go.transform.rotation = this.transform.rotation;
            go.transform.SetParent(this.traceResultParent, true);
        }
    }

    private void OnPathTraceComplete()
    {
        this.transform.position = this.nodes[0];
    }
	
	public static Vector3[] GetPath(string requestedName){
		requestedName = requestedName.ToLower();
		if(paths.ContainsKey(requestedName)){
			return paths[requestedName].nodes.ToArray();
		}else{
			Debug.Log("No path with that name exists! Are you sure you wrote it correctly?");
			return null;
		}
	}
}

