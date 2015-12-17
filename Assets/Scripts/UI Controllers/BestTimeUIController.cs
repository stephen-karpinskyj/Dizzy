using UnityEngine;
using UnityEngine.UI;

public class BestTimeUIController : MonoBehaviour
{
    [SerializeField]
    private string message = "<color={0}> {1:+0.000;-0.000;-0.000}s</color>";

    [SerializeField]
    private Text text;

    [SerializeField]
    private string defaultDiffColor = "#cdcdcdff";

    [SerializeField]
    private string negativeDiffColor = "#ffa500ff";

    [SerializeField]
    private AudioSource source;

    private string currentColor;

    private void Start()
    {
        this.currentColor = this.defaultDiffColor;
    }

    private void Update()
    {
        var diffTime = PlayerPrefs.GetFloat("Diff");
        this.text.text = string.Format(this.message, this.currentColor, diffTime);
    }

    private void PrevScoreChange()
    {
        if (PlayerPrefs.GetFloat("Diff") < 0f)
        {
            this.currentColor = this.negativeDiffColor;
            this.source.Play();
        }
        else
        {
            this.currentColor = this.defaultDiffColor;
        }
    }
}
