using UnityEngine;

public class UICamera : MonoBehaviour
{
    [SerializeField]
    private Camera cam;
    
    private static UICamera instance;
    
    public static UICamera Instance
    {
        get { return instance; }
    }
    
    public Camera Camera
    {
        get { return this.cam; }
    }
    
    private void Awake()
    {
        Debug.Assert(!instance);
        
        instance = this;
    }
}
