using UnityEngine;

public class CanvasUIController : MonoBehaviour
{
    [SerializeField]
    private AudioSource source;

    [SerializeField]
    private AudioClip winClip;

    [SerializeField]
    private AudioClip newBestClip;

    [SerializeField]
    private AudioClip clickClip;

    [SerializeField]
    private AudioClip launchClip;

    [SerializeField]
    private int frameRate = 60;

    private void Awake()
    {        
        Debug.Assert(this.source, this);
        Debug.Assert(this.winClip, this);

        Application.targetFrameRate = this.frameRate;

        LevelManager.Instance.Initialise();
    }


    #region Broadcast


    private void LevelWin(bool newBestTime)
    {
        this.source.clip = newBestTime ? this.newBestClip : this.winClip;
        AudioManager.Instance.Play(this.source);
    }

    private void LevelStop()
    {
        this.Show(true);
    }

    private void LevelStart()
    {
        this.Show(false);

        this.source.clip = this.launchClip;
        AudioManager.Instance.Play(this.source);

    }


    #endregion


    public void PlayClickSound()
    {
        this.source.clip = this.clickClip;
        AudioManager.Instance.Play(this.source);
    }

    private void Show(bool show)
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            var t = this.transform.GetChild(i);
            t.gameObject.SetActive(show);
        }
    }
}
