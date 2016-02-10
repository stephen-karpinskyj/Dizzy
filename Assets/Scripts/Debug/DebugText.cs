using UnityEngine;

public class DebugText : MonoBehaviour
{
    private static GUIStyle TextStyle;
    
    private Vector3 worldPos;
    public string Text { get; set; }

    private bool lastsForever;
    private float destroyTime;

    public void Move(Vector3 worldPos)
    {
        //if (!Debug.isDebugBuild)
        {
            //return;
        }

        this.worldPos = worldPos;
    }

    private void Update()
    {
        //if (!Debug.isDebugBuild)
        {
            //return;
        }

        if (lastsForever)
        {
            return;
        }
        
        if (Time.time >= this.destroyTime)
        {
            Object.Destroy(this.gameObject);
        }
    }

    private void OnGUI()
    {
        //if (!Debug.isDebugBuild)
        {
            //return;
        }

        var screenPoint = Camera.main.WorldToScreenPoint(this.worldPos);
        var rect = new Rect(screenPoint.x, Screen.height - screenPoint.y, 500f, 500f); 
        GUI.Label(rect, this.Text, TextStyle);
    }

    static DebugText()
    {
        TextStyle = new GUIStyle();
        TextStyle.fontSize = 20;
        TextStyle.normal.textColor = Color.white;
    }

    public static DebugText Draw(Vector3 worldPos, string text, float duration = 0f)
    {
        var go = new GameObject("Text");
        var debug = go.AddComponent<DebugText>();

        //if (!Debug.isDebugBuild)
        {
            //return debug;
        }

        if (Mathf.Approximately(duration, 0f))
        {
            debug.lastsForever = true;
        }
        else
        {
            debug.destroyTime = Time.time + duration;
        }

        debug.worldPos = worldPos;
        debug.Text = text;

        return debug;
    }
}
