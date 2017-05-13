using UnityEngine;

/// <summary>
/// Pool initialized with transforms of direct descendants.
/// </summary>
public class TransformPool : GenericPool<Transform>
{
    public TransformPool(Transform transform)
        : base(transform.GetDirectDescendents())
    {
    }
}
