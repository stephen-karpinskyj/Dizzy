using System;
using UnityEngine;

using Random = UnityEngine.Random;

/// <summary>
/// Conrols a junk element at run-time.
/// </summary>
public class JunkController : LevelObjectController
{
    #region Inspector


    [SerializeField]
    private Collider2D coll;

    [SerializeField]
    private RandomJunkElement[] randomElements;

    [SerializeField]
    private ParticleSystem sparkParticles;

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


    #endregion


    #region Fields


    private RandomJunkElement chosenElement;

    private float startPullTime;

    private bool isRunning = false;
    private bool isCollected = false;

    private float rotationAtAttraction;
    
    private Action<JunkController> onCollect;


    #endregion


    #region Properties


    public bool IsAttracted { get; private set; }

    public bool IsAttractable { get; set; }

    public RandomJunkElement ChosenElement
    {
        get { return this.chosenElement; }
    }


    #endregion


    #region Unity


    protected override void Awake()
    {
        base.Awake();
        
        Debug.Assert(this.sparkParticles, this);
        Debug.Assert(this.source, this);
    }

    private void FixedUpdate()
    {
        if (!this.isRunning || !this.IsAttracted)
        {
            return;
        }

        var dir = (Vector2)(GameManager.Instance.Ship.transform.position - this.transform.position).normalized; // Ignore z

        var shipMultipler = 1f + GameManager.Instance.Ship.SpeedPercentage * this.shipSpeedInfluence;
        var curveValue = this.gravityCurve.Evaluate((Time.time - this.startPullTime) / this.gravityCurveDuration);
        var multiplier = Mathf.Lerp(this.speedMultiplierRange.x, this.speedMultiplierRange.y, curveValue);
        var speed = multiplier * shipMultipler * Time.fixedDeltaTime;

        var velocity = (Vector3)dir * speed;

        var pos = this.transform.position;
        pos += velocity;
        this.transform.position = pos;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!this.isRunning || this.isCollected || !this.IsAttractable)
        {
            return;
        }

        if (other.tag == this.junkDestroyerTag && !this.isCollected)
        {
            this.BecomeCollected();
            this.onCollect(this);
        }
        else if (other.tag == this.junkPullerTag && !this.IsAttracted)
        {
            this.StartPulling();
        }
    }


    #endregion


    #region LeveObjectController


    public override void OnLevelStop()
    {        
        base.OnLevelStop();
        
        this.isRunning = false;

        this.isCollected = false;
        this.startPullTime = 0f;

        this.IsAttracted = false;
        this.coll.enabled = true;
        
        this.transform.localPosition = Vector3.zero;
        this.transform.eulerAngles = new Vector3(Random.Range(0, 360f), Random.Range(0, 360f), Random.Range(0, 360f));
        
        this.Randomize();
        this.StartCoroutine(this.ShowCoroutine(this.chosenElement.Rend));
    }

    public override void OnLevelStart()
    {
        base.OnLevelStart();
        
        this.isRunning = true;
    }


    #endregion
    
    
    #region Public
    
    
    public void Initialize(Action<JunkController> onCollect)
    {
        this.onCollect = onCollect;
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
            e.Rend.enabled = false;
        }
    }

    private void StartPulling()
    {
        Debug.Assert(!this.IsAttracted);

        this.IsAttracted = true;
        this.startPullTime = Time.time;

        var vel = (Vector2)(this.transform.position - GameManager.Instance.Ship.transform.position).normalized;
        GameManager.Instance.Ship.AddImpulseForce(vel * this.attractForceMagnitude);
        
        this.transform.parent = null;

        this.rotationAtAttraction = Vector2.down.SignedAngle(vel);
    }

    private void BecomeCollected()
    {
        Debug.Assert(!this.isCollected);

        this.isCollected = true;
        this.coll.enabled = false;

        this.source.clip = this.deathClips[Random.Range(0, this.deathClips.Length)];
        this.source.volume = this.deathVolume;
        this.source.pitch = 1f + this.deathPitchMultiplier * GameManager.Instance.Multiplier.CurrentMultiplier;
        AudioManager.Instance.Play(this.source);

        this.chosenElement.Rend.enabled = false;

        this.transform.eulerAngles = Vector3.forward * this.rotationAtAttraction;

        this.sparkParticles.Play();

        GameManager.Instance.Multiplier.Increment();
    }


    #endregion
}
