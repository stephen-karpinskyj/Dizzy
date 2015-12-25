using UnityEngine;

public static class Vector2Extensions
{
    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        return Quaternion.AngleAxis(degrees, Vector3.forward) * v;
    }

    public static float SignedAngle(this Vector2 v, Vector2 to)
    {
        var angle = Vector2.Angle(v, to);
        var cross = Vector3.Cross(v, to);

        return angle * Mathf.Sign(cross.z);
    }
}