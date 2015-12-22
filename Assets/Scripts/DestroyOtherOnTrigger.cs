using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DestroyOtherOnTrigger : MonoBehaviour
{
    [SerializeField]
    private string otherPrefixFilter;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.name.StartsWith(this.otherPrefixFilter))
        {
            return;
        }

        Object.Destroy(other.gameObject);
    }
}
