using UnityEngine;

public class GameStarter : MonoBehaviour
{
    private void Awake()
    {
        GameManager.Instance.Initialise();
    }
}
