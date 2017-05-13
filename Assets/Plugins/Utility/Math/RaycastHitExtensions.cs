using UnityEngine;

public static class RaycastHitExtensions
{
    public static GameObject GetGameObject(this RaycastHit hit)
    {
        return hit.transform == null ? null : hit.transform.gameObject;
    }
}
