using UnityEngine;

public class DebugLine : MonoBehaviour
{
    private static Material Mat;

    private bool lastsForever;
    private float destroyTime;

    private LineRenderer line;

    public void Move(Vector3 a, Vector3 b)
    {
        if (!Debug.isDebugBuild)
        {
            return;
        }

        this.line.SetPosition(0, a);
        this.line.SetPosition(1, b);
    }

    private void Update()
    {
        if (!Debug.isDebugBuild)
        {
            return;
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

    static DebugLine()
    {
        Mat = new Material(Shader.Find("Diffuse"));
    }

    public static DebugLine Draw(Vector3 a, Vector3 b, Color c, float duration = 0f)
    {
        var go = new GameObject("Line");
        var debug = go.AddComponent<DebugLine>();

        if (!Debug.isDebugBuild)
        {
            return debug;
        }

        if (Mathf.Approximately(duration, 0f))
        {
            debug.lastsForever = true;
        }
        else
        {
            debug.destroyTime = Time.time + duration;
        }

        var line = go.AddComponent<LineRenderer>();
        line.material = Mat;
        line.material.color = c;
        line.positionCount = 2;
        line.startWidth = 0.03f;
        line.endWidth = 0.03f;

        debug.line = line;
        debug.Move(a, b);

        return debug;
    }
}
