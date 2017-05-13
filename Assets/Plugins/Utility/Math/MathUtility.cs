using UnityEngine;

public static class MathUtility
{
    /// <remarks>Based on: http://answers.unity3d.com/questions/50391/how-to-round-a-float-to-2-dp.html</remarks>
    public static float Truncate(float f, int numDecimalPlaces)
    {
        var pow10 = Mathf.Pow(10, numDecimalPlaces);
        return Mathf.Round(f * pow10) / pow10;
    }

    /// <summary>
    /// Clamps and normalizes <paramref name="angle"/> in degrees between
    /// <paramref name="min"/> and <paramref name="max"/> degrees.
    /// </summary>
    public static float ClampAngle(float angle, float min, float max)
    {
        angle = NormalizeAngle(angle);

        min = NormalizeAngle(min);
        max = NormalizeAngle(max);

        return Mathf.Clamp(angle, min, max);
    }

    /// <summary>
    /// Normalizes <paramref name="angle"/> in degrees between -180 and 180.
    /// </summary>
    public static float NormalizeAngle(float angle)
    {
        angle %= 360f;

        if (angle > 180f)
        {
            angle -= 360f;
        }
        else if (angle < -180f)
        {
            angle += 360f;
        }

        return angle;
    }

    /// <remarks>Based on: http://forum.unity3d.com/threads/how-do-i-find-the-closest-point-on-a-line.340058/</remarks>
    public static Vector2 NearestPointOnLineSegment(Vector2 start, Vector2 end, Vector2 point)
    {
        var line = (end - start);
        var len = line.magnitude;
        line.Normalize();

        var v = point - start;
        var d = Vector3.Dot(v, line);
        d = Mathf.Clamp(d, 0f, len);
        return start + line * d;
    }

    /// <remarks>Based on: http://forum.unity3d.com/threads/moving-an-object-on-a-specific-arc.387757/</remarks>
    public static float CircleArcDistanceToAngleOffset(float distance, float circleRadius)
    {
        return (distance / circleRadius) * Mathf.Rad2Deg;
    }

    /// <remarks>Source: http://answers.unity3d.com/questions/823090/equivalent-of-degree-to-vector2-in-unity.html#answer-823216</remarks>
    public static Vector2 ToHeading(float degrees)
    {
        return Quaternion.AngleAxis(degrees, Vector3.forward) * Vector2.up;
    }

    public static float PercentBetween(float min, float max, float value)
    {
        return Mathf.Clamp01((value - min) / (max - min));
    }      

    /// <remarks>Based on: http://stackoverflow.com/questions/217578/how-can-i-determine-whether-a-2d-point-is-within-a-polygon</remarks>
    public static bool PolygonContainsPoint(Vector2[] polygon, Vector2 point)
    {
        var bounds = CalculateBounds(polygon);

        var p = point;
        var p2 = new Vector2(bounds.max.x + Mathf.Epsilon, point.y);

        return CountIntersections(p, p2, polygon) % 2 == 1;
    }

    private static Bounds CalculateBounds(Vector2[] points)
    {
        var min = new Vector2(float.MinValue, float.MinValue);
        var max = new Vector2(float.MaxValue, float.MaxValue);

        foreach (var p in points)
        {
            min.x = Mathf.Max(min.x, p.x);
            max.x = Mathf.Min(max.x, p.x);
            min.y = Mathf.Max(min.y, p.y);
            max.y = Mathf.Min(max.y, p.y);
        }

        var size = new Vector2(max.x - min.x, max.y - min.y);
        var center = max - size / 2f;

        return new Bounds(center, size);
    }

    private static int CountIntersections(Vector2 p, Vector2 p2, Vector2[] polygon)
    {
        var numIntersections = 0;

        for (var i = 0; i <= polygon.Length; i++)
        {
            var q = polygon[i == 0 ? polygon.Length - 1 : i - 1];
            var q2 = polygon[i == polygon.Length ? 0 : i];

            if (Intersects(p, p2, q, q2))
            {
                numIntersections++;
            }
        }

        return numIntersections;
    }

    /// <remarks>Based on: http://www.codeproject.com/Tips/862988/Find-the-Intersection-Point-of-Two-Line-Segments</remarks>
    public static bool Intersects(Vector2 p, Vector2 p2, Vector2 q, Vector2 q2)
    {
        var r = p2 - p;
        var s = q2 - q;
        var rXs = r.Cross(s);
        var qpXr = (q - p).Cross(r);

        // If r x s = 0 and (q - p) x r = 0, then the two lines are collinear.
        if (Mathf.Approximately(rXs, 0f) && Mathf.Approximately(qpXr, 0f))
        {
            // 1. If either  0 <= (q - p) * r <= r * r or 0 <= (p - q) * s <= * s
            // then the two lines are overlapping,
            var qpMr = (q - p).Multiply(r);
            var pqMs = (p - q).Multiply(s);
            var rMr = r.Multiply(r);
            var sMs = s.Multiply(s);
            if ((0 <= qpMr.x && 0 <= qpMr.y && qpMr.x <= rMr.x && qpMr.y <= rMr.y) ||
                (0 <= pqMs.x && 0 <= pqMs.y && pqMs.x <= sMs.x && pqMs.y <= sMs.y))
            {
                return true;
            }

            // 2. If neither 0 <= (q - p) * r = r * r nor 0 <= (p - q) * s <= s * s
            // then the two lines are collinear but disjoint.
            // No need to implement this expression, as it follows from the expression above.
            return false;
        }

        // 3. If r x s = 0 and (q - p) x r != 0, then the two lines are parallel and non-intersecting.
        if (Mathf.Approximately(rXs, 0f) && !Mathf.Approximately(qpXr, 0f))
        {
            return false;
        }

        // t = (q - p) x s / (r x s)
        var qpXs = (q - p).Cross(s);
        var t = qpXs / rXs;

        // u = (q - p) x r / (r x s)
        var u = qpXr / rXs;

        // 4. If r x s != 0 and 0 <= t <= 1 and 0 <= u <= 1
        // the two line segments meet at the point p + t r = q + u s.
        if (!Mathf.Approximately(rXs, 0f) && (0 <= t && t <= 1) && (0 <= u && u <= 1))
        {
            // An intersection was found.
            return true;
        }

        // 5. Otherwise, the two line segments are not parallel but do not intersect.
        return false;
    }
}
