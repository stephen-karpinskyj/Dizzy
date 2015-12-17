using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class DestroyOnInvisible : MonoBehaviour
{
    [SerializeField]
    private GameObject destroyable;

    private void OnBecameInvisible()
    {
        Object.Destroy(this.destroyable);
    }
}
