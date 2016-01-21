using UnityEngine;

public class SpinningNode : LevelObjectNode
{
    [SerializeField]
    private Vector3 magnitude = Vector3.up;
    

    #region MonoBehaviour


    private void Update()
    {
        this.transform.localRotation = this.transform.localRotation * Quaternion.Euler(Time.smoothDeltaTime * this.magnitude);
    }
    
    
    #endregion
    

    public void SetMagnitude(Vector3 mag)
    {
        this.magnitude = mag;
    }
}
