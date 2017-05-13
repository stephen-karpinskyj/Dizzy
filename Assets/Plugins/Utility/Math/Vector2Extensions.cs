using UnityEngine;

public static class Vector2Extensions
{
    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        return Quaternion.AngleAxis(degrees, Vector3.forward) * v;
    }

    public static Vector2 RotateAround(this Vector2 point, Vector2 pivot, float degrees)
    {
        var offset = point - pivot;
        var rotatedDir = offset.normalized.Rotate(degrees);
        return pivot + rotatedDir * offset.magnitude;
    }

    public static float SignedAngle(this Vector2 from, Vector2 to)
    {
        var angle = Vector2.Angle(from, to);
        var cross = Vector3.Cross(from, to);

        return angle * Mathf.Sign(cross.z);
    }

    public static float Cross(this Vector2 a, Vector2 b)
    {
        return a.x * b.y - a.y * b.x;
    }

    public static Vector2 Multiply(this Vector2 a, Vector2 b)
    {
        a.Scale(b);
        return a;
    }
}