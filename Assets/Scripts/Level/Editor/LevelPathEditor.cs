using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelPath))]
public class LevelPathEditor : Editor
{
	LevelPath _target;
	GUIStyle style = new GUIStyle();
	public static int count = 0;
	
	void OnEnable(){
		//i like bold handle labels since I'm getting old:
		style.fontStyle = FontStyle.Bold;
		style.normal.textColor = Color.white;
		_target = (LevelPath)target;
		
		//lock in a default path name:
		if(!_target.initialized){
			_target.initialized = true;
			_target.pathName = "New Path " + ++count;
			_target.initialName = _target.pathName;
		}
	}
	
	public override void OnInspectorGUI(){		
		//path name:
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Path Name");
		_target.pathName = EditorGUILayout.TextField(_target.pathName);
		EditorGUILayout.EndHorizontal();
		
		if(_target.pathName == ""){
			_target.pathName = _target.initialName;
		}
		
		//path color:
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Path Color");
		_target.pathColor = EditorGUILayout.ColorField(_target.pathColor);
		EditorGUILayout.EndHorizontal();
		
		//exploration segment count control:
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Node Count");
		_target.nodeCount =  Mathf.Clamp(EditorGUILayout.IntSlider(_target.nodeCount, 0, 10), 2,100);
		EditorGUILayout.EndHorizontal();
		
		//add node?
		if(_target.nodeCount > _target.nodes.Count){
			for (int i = 0; i < _target.nodeCount - _target.nodes.Count; i++) {
				_target.nodes.Add(Vector3.zero);	
			}
		}
	
		//remove node?
		if(_target.nodeCount < _target.nodes.Count){
			if(EditorUtility.DisplayDialog("Remove path node?","Shortening the node list will permantently destory parts of your path. This operation cannot be undone.", "OK", "Cancel")){
				int removeCount = _target.nodes.Count - _target.nodeCount;
				_target.nodes.RemoveRange(_target.nodes.Count-removeCount,removeCount);
			}else{
				_target.nodeCount = _target.nodes.Count;	
			}
		}
				
		//node display:
		EditorGUI.indentLevel = 4;
		for (int i = 0; i < _target.nodes.Count; i++) {
			_target.nodes[i] = EditorGUILayout.Vector3Field("Node " + (i+1), _target.nodes[i]);
        }

        EditorGUILayout.Space();

        // Trace
        {
            EditorGUI.indentLevel = 0;
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Trace", EditorStyles.miniButton, GUILayout.Width(40f)))
            {
                var points = LevelPath.GetPath(this._target.pathName);
                var args = iTween.Hash("path", points, "speed", this._target.traceSpeed, "onstart", "OnPathTraceStart", "onupdate", "OnPathTraceUpdate", "oncomplete", "OnPathTraceComplete");
                iTween.MoveTo(this._target.gameObject, args);
            }
            
            GUILayout.Space(8f);
            
            GUILayout.Label("Target");
            _target.traceOutputParent = EditorGUILayout.ObjectField(_target.traceOutputParent, typeof(Transform), true) as Transform;
            
            GUILayout.Space(8f);
            
            GUILayout.Label("Type");
            _target.traceOutputNode = EditorGUILayout.ObjectField(_target.traceOutputNode, typeof(LevelObjectNode), false) as LevelObjectNode;
                        
            EditorGUILayout.EndHorizontal();
        }
        
        //update and redraw:
        if(GUI.changed){
            EditorUtility.SetDirty(_target);			
        }
	}
	
	void OnSceneGUI(){
		if(_target.enabled) { // dkoontz
			if(_target.nodes.Count > 0){
				//allow path adjustment undo:
				Undo.RecordObject(_target,"Adjust iTween Path");
				
				//path begin and end labels:
				Handles.Label(_target.nodes[0], "'" + _target.pathName + "' Begin", style);
				Handles.Label(_target.nodes[_target.nodes.Count-1], "'" + _target.pathName + "' End", style);
				
				//node handle display:
				for (int i = 0; i < _target.nodes.Count; i++) {
					_target.nodes[i] = Handles.PositionHandle(_target.nodes[i], Quaternion.identity);
				}	
			}
		} // dkoontz
	}
}