using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WallController : MonoBehaviour
{
    [SerializeField]
    private string destroyerTag = "Player";
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag != destroyerTag)
        {
            return;
        }

        GameManager.Instance.HandleOutOfBounds();
    }
}
