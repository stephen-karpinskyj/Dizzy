using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LevelPath : MonoBehaviour
{
	public string pathName ="";
	public Color pathColor = Color.cyan;
	public List<Vector3> nodes = new List<Vector3>(){Vector3.zero, Vector3.zero};
	public int nodeCount;
	public static Dictionary<string, LevelPath> paths = new Dictionary<string, LevelPath>();
	public bool initialized = false;
	public string initialName = "";
    public Transform traceOutputParent;
    public LevelObjectNode traceOutputNode;
	
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
        for (int i = 0; i < this.traceOutputParent.childCount; i++)
        {
            Object.DestroyImmediate(this.traceOutputParent.GetChild(i).gameObject);
        }

        this.transform.position = this.nodes[0];
        this.lastPointTraced = Vector3.zero;
    }

    private void OnPathTraceUpdate()
    {
        if (Vector3.Distance(this.transform.position, this.lastPointTraced) > this.traceDistance)
        {
            this.lastPointTraced = this.transform.position;

            var obj = GameObjectUtility.InstantiatePrefab(this.traceOutputNode, this.traceOutputParent);
            obj.name = this.traceOutputNode.GetType().Name;
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

