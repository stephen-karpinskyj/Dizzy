using UnityEngine;

public class DisableBehaviourOnPlay : MonoBehaviour
{
    [SerializeField]
    private MonoBehaviour behaviour;

    [SerializeField]
    private bool showOnlyOnWin = false;

    private void LevelWin()
    {
        this.behaviour.enabled = true;
    }

    private void LevelStart()
    {
        this.behaviour.enabled = !this.showOnlyOnWin;
    }

    private void TimeStart()
    {
        this.behaviour.enabled = false;
    }

}
