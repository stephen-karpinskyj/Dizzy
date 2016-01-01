using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TimerUIController : MonoBehaviour
{
    [SerializeField]
    private string message = "<color={0}>{1}s ({2})</color>";

    [SerializeField]
    private Text text;

    [SerializeField]
    private float defaultTargetTime = 40f;

    [SerializeField]
    private float panicStartTime = 5f;

    [SerializeField]
    private string countdownColor = "#323232ff";

    [SerializeField]
    private string panicColor = "#ff0000ff";

    [SerializeField]
    private string countupColor = "#32323255";

    [SerializeField]
    private string newHighScoreColor = "#ffa500ff";

    [SerializeField]
    private AudioSource source;

    private string currentColor;
    private bool isChangingColor = false;

    private float startTime;
    private bool isRunning = false;

    private bool isCountingDownAudio = false;

    private void LevelStart()
    {
        this.currentColor = this.countdownColor;
        this.isRunning = false;

        this.StopCoroutine("CountdownSound");
        this.isCountingDownAudio = false;
    }

    private void TimeStart()
    {
        this.startTime = Time.time;

        if (!PlayerPrefs.HasKey("Time"))
        {
            PlayerPrefs.SetFloat("Time", this.defaultTargetTime);
        }

        this.isRunning = true;
    }

    private void LevelWin()
    {
        var timePassed = Time.time - this.startTime;
        var bestTime = PlayerPrefs.GetFloat("Time", this.defaultTargetTime);
        var timeDiff = timePassed - bestTime;

        if (timePassed < bestTime)
        {
            PlayerPrefs.SetFloat("Time", timePassed);
            this.StartCoroutine(this.HighScoreChange());
        }

        PlayerPrefs.SetFloat("Diff", timeDiff);
        Broadcast.SendMessage("PrevScoreChange");

        this.currentColor = this.countdownColor;
        this.isRunning = false;

        this.StopCoroutine("CountdownSound");
        this.isCountingDownAudio = false;
    }

    private void Update()
    {
        var bombs = Object.FindObjectsOfType<BombController>();

        if (isRunning)
        {
            var timeLeftFloat = PlayerPrefs.GetFloat("Time", this.defaultTargetTime) - (Time.time - this.startTime);
            var timeLeft = Mathf.Floor(timeLeftFloat);

            if (timeLeft < 0)
            {
                this.text.text = string.Format(this.message, this.countupColor, -timeLeft, bombs.Length);
            } 
            else if (timeLeft <= this.panicStartTime)
            {
                if (!this.isCountingDownAudio) {
                    this.StartCoroutine("CountdownSound");
                    this.isCountingDownAudio = true;
                }

                this.text.text = string.Format(this.message, timeLeftFloat % 1f > 0.5f ? this.panicColor : this.countdownColor, timeLeftFloat.ToString("f1"), bombs.Length);
            }
            else
            {
                this.text.text = string.Format(this.message, this.countdownColor, timeLeft, bombs.Length);
            }

            if (bombs.Length == 1 && bombs[0].HasExploded)
            {
                foreach (var b in bombs)
                {
                    Object.Destroy(b.gameObject);
                }
                
                Broadcast.SendMessage("LevelWin");
            }
        }
        else
        {
            var bestTime = PlayerPrefs.GetFloat("Time", this.defaultTargetTime);
            this.text.text = string.Format(this.message, this.currentColor, bestTime.ToString("f3"), bombs.Length);
        }
    }

    private IEnumerator CountdownSound()
    {
        for (var i = 0; i <= 5; i++)
        {
            this.source.Play();
            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator HighScoreChange()
    {
        if (this.isChangingColor)
        {
            yield break;
        }

        this.isChangingColor = true;

        for (var i = 0; i < 3; i++)
        {
            this.currentColor = this.countdownColor;
            yield return new WaitForSeconds(0.25f);

            this.currentColor = this.newHighScoreColor;
            yield return new WaitForSeconds(0.25f);
        }

        this.currentColor = this.countdownColor;
        this.isChangingColor = false;
    }

}
