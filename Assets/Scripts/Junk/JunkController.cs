using System.Collections;
using UnityEngine;

/// <summary>
/// Conrols a junk element at run-time.
/// </summary>
public class JunkController : MonoBehaviour
{
    #region Inspector


    [SerializeField]
    private Collider2D coll;

    [SerializeField]
    private RandomJunkElement[] randomElements;

    [SerializeField]
    private ParticleSystem explosionParticles;

    [SerializeField]
    private AudioSource source;

    [SerializeField]
    private AudioClip[] deathClips;

    [SerializeField]
    private float deathVolume = 0.1f;

    [SerializeField]
    private float deathPitchMultiplier = 0.02f;

    [SerializeField]
    private string junkPullerTag = "JunkPuller";

    [SerializeField]
    private string junkDestroyerTag = "JunkDestroyer";

    [SerializeField]
    private float attractForceMagnitude = 20f;

    [SerializeField]
    private AnimationCurve gravityCurve;

    [SerializeField]
    private float gravityCurveDuration = 0.2f;

    [SerializeField]
    private float shipSpeedInfluence = 2f;

    [SerializeField]
    private Vector2 speedMultiplierRange = new Vector2(20f, 80f);

    [SerializeField]
    private Vector2 showPositionRange = new Vector2(-4f, 4f);

    [SerializeField]
    private float maxShowDuration = 0.25f;

    [SerializeField]
    private Vector2 showDurationOffset = new Vector2(0f, 0.25f);

    [SerializeField]
    private AnimationCurve scaleCurve;

    [SerializeField]
    private float scaleDuration = 0.1f;


    #endregion


    #region Fields



    private Vector3 initialPos;
    private Vector3 initialScale;

    private RandomJunkElement chosenElement;

    private float startPullTime;

    private bool isRunning = false;
    private bool isPulling = false;
    private bool isExploded = false;

    private float rotationAtAttraction;


    #endregion


    #region Properties


    public RandomJunkElement ChosenElement
    {
        get { return this.chosenElement; }
    }


    #endregion


    #region Unity


    private void Awake()
    {
        Debug.Assert(this.explosionParticles, this);
        Debug.Assert(this.source, this);

        this.initialPos = this.transform.position;
        this.initialScale = this.transform.localScale;

        this.StopRunning();
    }

    private void FixedUpdate()
    {
        if (!this.isRunning || !this.isPulling)
        {
            return;
        }

        var dir = (Vector2)(LevelManager.Instance.Ship.transform.position - this.transform.position).normalized; // Ignore z

        var curveValue = this.gravityCurve.Evaluate((Time.time - this.startPullTime) / this.gravityCurveDuration);
        var shipMultipler = 1f + LevelManager.Instance.Ship.SpeedPercentage * this.shipSpeedInfluence;
        var multiplier = Mathf.Lerp(this.speedMultiplierRange.x, this.speedMultiplierRange.y, curveValue);
        var speed = multiplier * shipMultipler * Time.fixedDeltaTime;

        var velocity = (Vector3)dir * speed;

        var pos = this.transform.position;
        pos += velocity;
        this.transform.position = pos;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!this.isRunning || this.isExploded)
        {
            return;
        }

        if (other.tag == this.junkDestroyerTag && !this.isExploded)
        {
            this.Explode();
            LevelManager.Instance.HandlePickup(this);
        }
        else if (other.tag == this.junkPullerTag && !this.isPulling)
        {
            this.StartPulling();
        }
    }


    #endregion


    #region Public


    public void StopRunning()
    {
        this.isRunning = false;

        this.isExploded = false;
        this.startPullTime = 0f;

        this.isPulling = false;
        this.coll.enabled = true;

        this.transform.position = this.initialPos;
        this.transform.eulerAngles = new Vector3(Random.Range(0, 360f), Random.Range(0, 360f), Random.Range(0, 360f));

        this.Randomize();
        this.StartCoroutine(this.ShowCoroutine());
    }

    public void StartRunning()
    {
        if (this.isRunning)
        {
            return;
        }

        this.isRunning = true;
    }


    #endregion


    #region Private


    private void Randomize()
    {
        var totalOdds = 0;

        foreach (var e in this.randomElements)
        {
            totalOdds += e.OddsRatio;
        }

        var r = Random.Range(0, totalOdds);
        var prev = 0;
        var curr = 0;

        foreach (var e in this.randomElements)
        {
            curr += e.OddsRatio;

            if (r >= prev && r < curr)
            {
                this.chosenElement = e;
                break;
            }

            prev = curr;
        }

        foreach (var e in this.randomElements)
        {
            e.SetEnabled(false);
        }
    }

    private void StartPulling()
    {
        Debug.Assert(!this.isPulling);

        this.isPulling = true;
        this.startPullTime = Time.time;

        var vel = (Vector2)(this.transform.position - LevelManager.Instance.Ship.transform.position).normalized;
        LevelManager.Instance.Ship.AddImpulseForce(vel * this.attractForceMagnitude);

        this.rotationAtAttraction = Vector2.down.SignedAngle(vel);
    }

    private void Explode()
    {
        Debug.Assert(!this.isExploded);

        this.isExploded = true;
        this.coll.enabled = false;

        this.source.clip = this.deathClips[Random.Range(0, this.deathClips.Length)];
        this.source.volume = this.deathVolume;
        this.source.pitch = 1f + this.deathPitchMultiplier * MultiplierController.Instance.CurrentMultiplier;
        AudioManager.Instance.Play(this.source);

        this.chosenElement.SetEnabled(false);

        this.transform.eulerAngles = Vector3.forward * this.rotationAtAttraction;

        this.explosionParticles.Play();

        MultiplierController.Instance.Increment();
    }


    #endregion


    #region Coroutines


    private IEnumerator ShowCoroutine()
    {
        var x = this.transform.position.x - this.showPositionRange.x;
        var t = x / (this.showPositionRange.y - this.showPositionRange.x);

        yield return new WaitForSeconds(this.maxShowDuration * t + Random.Range(this.showDurationOffset.x, this.showDurationOffset.y));

        this.transform.localScale = this.initialScale;
        this.chosenElement.SetEnabled(true);

        var time = Time.time;
        var progress = 0f;

        while (progress < 1f) 
        {
            progress = Mathf.Clamp01((Time.time - time) / this.scaleDuration);
            this.transform.localScale = this.initialScale * this.scaleCurve.Evaluate(progress);
            yield return null;
        }
    }


    #endregion
}
