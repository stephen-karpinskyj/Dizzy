using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineController : LevelObjectController
{
    private enum State
    {
        Off = 0,
        Waiting,
        Exploded,
    }
    
    [SerializeField]
    private Renderer rend;
    
    [SerializeField]
    private string colourProperty = "_EmissionColor";
    
    [SerializeField]
    private float lightOnSpeed = 5f;
    
    [SerializeField]
    private float lightOffSpeed = 15f;
    
    [SerializeField]
    private float lightBlinkWaitDuration = 0.3f;
    
    [SerializeField]
    private string activatorTag;
    
    [SerializeField]
    private float explosionForce = 100f;
    
    [SerializeField]
    private float explosionShakeDuration = 0.2f;
    
    [SerializeField]
    private float explosionShakeMagnitude = 0.2f;

    [SerializeField]
    private float explosionDelay = 4/60f;
    
    [SerializeField]
    private Color blinkingColor;
    
    [SerializeField]
    private Color explodingColor;
    
    [SerializeField]
    private ParticleSystem explosionParticles;
    
    [SerializeField]
    private Transform junkParent;
    
    [SerializeField]
    private float junkPusherDuration = 0.2f;
    
    [SerializeField]
    private float junkPusherDistance = 0.5f;
    
    [SerializeField]
    private AnimationCurve junkPusherCurve;

    private State currState;
    
    private float currLight;
    private float targetLight;
    private float lastLightOffTime;
    
    private Color lightColour;
    
    private Dictionary<JunkPusher, JunkController> junkPushers;
    
    //private DebugText debugText;
    
    
    #region MonoBehaviour
    
    
    protected override void Awake()
    {
        base.Awake();
        
        this.lightColour = Color.black;
    }
    
    private void Start()
    {
        //this.debugText = DebugText.Draw(this.transform.position + Vector3.up * 1f, string.Empty);
    }
    
    private void Update()
    {
        if (this.currState == State.Exploded)
        {
            return;
        }
        
        var diff = this.targetLight - this.currLight;
        
        if (!Mathf.Approximately(diff, 0f))
        {
            var dir = Mathf.Sign(diff);
            var speed = (dir > 0) ? this.lightOnSpeed : this.lightOffSpeed;
            this.currLight += dir * speed * Time.deltaTime;
            
            this.UpdateLight(true);
        }
        
        if (Mathf.Approximately(this.currLight, 0f))
        {
            var isFinishedWaiting = (Time.time - this.lastLightOffTime) >= this.lightBlinkWaitDuration;
        
            if (isFinishedWaiting)
            {
                this.targetLight = 1f;
                this.lastLightOffTime = Time.time;
            }
        }
        else if (Mathf.Approximately(currLight, 1f))
        {
            this.targetLight = 0f;
        }
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag != this.activatorTag)
        {
            return;
        }
        
        if (this.currState == State.Waiting)
        {        
            if (GameManager.Instance.Ship.IsThrusting)
            {
                this.ChangeState(State.Exploded);
            }
        }
    }
    
    
    #endregion
    
    
    #region LeveObjectController


    public override void OnLevelStop()
    {        
        base.OnLevelStop();
        
        this.ChangeState(State.Off);
        
        foreach (var kv in this.junkPushers)
        {
            kv.Key.StopRunning();
            
            kv.Value.transform.parent = kv.Key.transform;
            kv.Value.IsAttractable = false;
            kv.Value.OnLevelStop();
        }
        
        this.rend.enabled = false;
        this.StartCoroutine(this.ShowCoroutine(this.rend));
    }
    
    public override void OnLevelStart()
    {
        base.OnLevelStart();
        
        this.ChangeState(State.Waiting);
        
        foreach (var kv in this.junkPushers)
        {
            kv.Value.OnLevelStart();
        }
    }
    
    
    #endregion
    
    
    public void Initialise(List<JunkController> junk)
    {
        this.junkPushers = new Dictionary<JunkPusher, JunkController>();
        
        var rot = 0f;
        var interval = 360f / junk.Count; 
        
        foreach (var j in junk)
        {
            var p = GameObjectUtility.InstantiateBehaviour<JunkPusher>(this.junkParent);
            p.transform.localPosition = Vector3.zero;
            p.transform.localRotation = Quaternion.identity;
            
            p.Initialise(j, rot, this.junkPusherDuration, this.junkPusherDistance, this.junkPusherCurve);
            
            this.junkPushers[p] = j;
            rot += interval;
        }
    }
    
    private void ChangeState(State state)
    {        
        if (this.currState == state)
        {
            return;
        }
        
        this.currState = state;
        //this.debugText.Text = state.ToString();
        
        switch (state)
        {
            case State.Off:
            case State.Waiting:
            {
                this.Reinitialise();
            }
            break;
            
            case State.Exploded:
            {
                this.StartCoroutine(this.ExplodeCoroutine());
            }
            break;
        }
    }
    
    private void Reinitialise()
    {
        this.StopAllCoroutines();
        
        this.currLight = 0f;
        this.UpdateLight(true);
        this.explosionParticles.Stop();

        this.rend.enabled = true;
    }
    
    private IEnumerator ExplodeCoroutine()
    {        
        this.currLight = 1f;
        this.UpdateLight(false);
        this.explosionParticles.Play();
        
        foreach (var kv in this.junkPushers)
        {
            kv.Key.StartRunning();
        }
        
        yield return new WaitForSeconds(this.explosionDelay);
                
        var ship = GameManager.Instance.Ship;
        ship.AddImpulseForce(ship.Direction * this.explosionForce, true);
        
        CameraController.Shake(this.explosionShakeDuration, this.explosionShakeMagnitude);
        
        this.rend.enabled = false;
    }
    
    private float UpdateLight(bool blinking)
    {
        this.currLight = Mathf.Clamp01(this.currLight);
        
        var r = this.currLight;
        var g = this.currLight;
        var b = this.currLight;
        
        if (blinking)
        {
            r *= this.blinkingColor.r;
            g *= this.blinkingColor.g;
            b *= this.blinkingColor.b;
        }
        else
        {
            r *= this.explodingColor.r;
            g *= this.explodingColor.g;
            b *= this.explodingColor.b;
        }
        
        this.lightColour.r = r;
        this.lightColour.g = g;
        this.lightColour.b = b;
        this.rend.material.SetColor(this.colourProperty, this.lightColour);
        
        return this.currLight;
    }
}
