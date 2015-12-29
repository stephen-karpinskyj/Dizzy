using UnityEngine;

public class CanvasUIController : MonoBehaviour
{
    [SerializeField]
    private AudioSource source;

    [SerializeField]
    private AudioClip winClip;

    [SerializeField]
    private int frameRate = 60;

    private void Awake()
    {        
        Debug.Assert(this.source, this);
        Debug.Assert(this.winClip, this);

        Application.targetFrameRate = this.frameRate;
    }

    private void LevelWin()
    {
        this.source.clip = this.winClip;
        this.source.Play();
    }
}
