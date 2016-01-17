using UnityEngine;

/// <summary>
/// Level definition data.
/// </summary>
public class LevelData : MonoBehaviour
{
    [SerializeField]
    private string id;
    
    [SerializeField]
    private string displayName;
    
    [SerializeField]
    private float noviceTime;
    
    [SerializeField]
    private float proTime;
    
    public string Id
    {
        get { return this.id; }
    }
    
    public float NoviceTime
    {
        get { return this.noviceTime; }
    }
    
    public float ProTime
    {
        get { return this.proTime; }
    }
}
