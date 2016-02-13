using UnityEngine;

public class GameStarter : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.Initialise();
    }
}
