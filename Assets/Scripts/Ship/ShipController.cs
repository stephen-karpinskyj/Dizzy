using UnityEngine;

public class ShipController : MonoBehaviour
{
    #region Types


    private enum DoubleTapMode
    {
        Nothing = 0,

        Shield,
    }
    
    private enum TapType
    {
        None = 0,
        Left,
        Right,
        Both,
    }


    #endregion


    #region Inspector


    [Header("Thrust")]

    [SerializeField]
    private float maxSpeed = 7f;
    [SerializeField]
    private float maxOverdriveSpeed = 8.5f;
    [SerializeField]
    private float maxSpeedDecayRate = 0.1f;
    [SerializeField]
    private AnimationCurve boostMultiplierCurve;
    [SerializeField]
    private Vector2 boostForce = new Vector2(300f, 0f);
    [SerializeField]
    private Vector2 boostOverdriveForce = new Vector2(300f, 0f);
    [SerializeField]
    private Vector2 boostMultiplierRange = new Vector2(0f, 1f);

    [SerializeField]
    private AnimationCurve boostTimeMultiplierCurve;
    [SerializeField]
    private float boostTimeMultiplierDuration = 0.5f;
    [SerializeField]
    private Vector2 boostTimeMultiplierRange = new Vector2(0.5f, 1f);

    [SerializeField]
    private AnimationCurve velocityDecayCurve;
    [SerializeField]
    private Vector2 velocityDecayRange = new Vector2(0.5f, 0.3f);
    [SerializeField]
    private AnimationCurve velocityDecayCurveSpeedInfluence;
    [SerializeField]
    private Vector2 velocityDecaySpeedInfluenceRange = new Vector2(1f, 1.3f);

    [SerializeField]
    private AnimationCurve angularVelocityDecayCurve;
    [SerializeField]
    private float angularVelocityDecayDuration = 0.1f;
    [SerializeField]
    private Vector2 angularVelocityDecayRange = new Vector2(0.1f, 0.2f);


    [Header("Spin")]

    [SerializeField]
    private float maxAngularVelocity = 260f;
    [SerializeField]
    private AnimationCurve torqueCurve;
    [SerializeField]
    private float torque = 150f;
    [SerializeField]
    private float torqueCurveDuration = 0.5f;


    [Header("Launch")]

    [SerializeField]
    private float launchShakeDuration = 0.2f;
    [SerializeField]
    private float launchShakeMagnitude = 0.1f;
    [SerializeField]
    private Vector2 launchForce = new Vector2(0f, 120f);


    [Header("Other")]

    [SerializeField]
    private Transform centreTransform;

    [SerializeField]
    private DoubleTapMode doubleTapMode = DoubleTapMode.Nothing;

    [SerializeField]
    private Rigidbody2D rigidBody;

    [SerializeField]
    private Transform bodyTransform;

    [SerializeField]
    private TrailRenderer[] trails;

    [SerializeField]
    private ShipShieldController shieldController;

    [SerializeField]
    private GameObject beamParent;

    [SerializeField]
    private float baseTrailDuration = 0.1f;

    [SerializeField]
    private float dotProductMinBeforeTrails = 0.9f;

    [SerializeField]
    private float doubleTapDuration = 0.2f;

    [SerializeField]
    private float minTimeBetweenDoubleTaps = 0.2f;


    #endregion


    #region Fields


    private Vector3 initialPos;
    private Quaternion initialRot;

    private TapType tapType;

    private float timeHeld;
    private float timeReleased;
    private bool isTapping;
    private bool wasTapping;
    private float timeThrusting;
    private bool isThrusting;
    private bool wasThrusting;
    private bool isDoubleTapping;

    private float angularVelocityOnRelease;

    private bool isRunning;
    private bool isCCW;

    private float lastTapStartTime;
    private float lastDoubleTapTime;
    
    private float currMaxSpeed;


    #endregion


    #region Properties


    public float AngularSpeedPercentage
    {
        get { return this.rigidBody.angularVelocity / this.maxAngularVelocity; }
    }
    
    public float SpeedPercentage
    {
        get { return this.rigidBody.velocity.magnitude / this.maxSpeed; }
    }
    
    public Vector2 VelocityNormalized
    {
        get { return this.rigidBody.velocity.normalized; }
    }

    public bool InOverdrive
    {
        get { return this.currMaxSpeed > this.maxSpeed; }
    }

    public bool IsThrusting
    { 
        get { return this.isThrusting; }
    }

    public float TimeThrusting
    {
        get { return this.timeThrusting; }
    }

    public bool IsCCW
    {
        get { return this.isCCW; }
    }
    
    public Vector2 Direction
    {
        get { return this.transform.right; }
    }
    
    public Transform Trans
    {
        get { return this.centreTransform; }
    }


    #endregion


    #region MonoBehaviour


    private void Awake()
    {
        this.initialPos = this.transform.position;
        this.initialRot = this.transform.rotation;

        this.currMaxSpeed = this.maxSpeed;
    }

    private void Start()
    {
        GameManager.Instance.OnLevelStop += this.HandleLevelStop;
        GameManager.Instance.OnLevelStart += this.HandleLevelStart;
    }

    private void OnDestroy()
    {
        if (GameManager.Exists)
        {
            GameManager.Instance.OnLevelStop -= this.HandleLevelStop;
            GameManager.Instance.OnLevelStart -= this.HandleLevelStart;
        }
    }

    private void FixedUpdate()
    {
        if (!this.isRunning)
        {
            if (this.IsThrusting)
            {
                this.OnStopThrusting();
            }

            return;
        }
        
        // Input
        {
            this.isTapping = Input.anyKey;
            this.tapType = this.GetCurrentTapType();

            if (this.isTapping)
            {
                this.timeReleased = 0f;
                this.timeHeld += Time.fixedDeltaTime;
            }
            else
            {
                this.timeHeld = 0f;
                this.timeReleased += Time.fixedDeltaTime;
            }

            if (this.isThrusting)
            {
                this.timeThrusting += Time.fixedDeltaTime;
            }
            else
            {
                this.timeThrusting = 0f;
            }
            
            var canDoubleTap = (Time.time - this.lastDoubleTapTime) >= this.minTimeBetweenDoubleTaps;
            if (canDoubleTap && this.isTapping && !this.wasTapping)
            {
                var doubleTapped = (Time.time - this.lastTapStartTime) <= this.doubleTapDuration;
                if (doubleTapped)
                {
                    this.lastDoubleTapTime = Time.time;
                    this.isDoubleTapping = true;
                    this.OnStartDoubleTap();
                }

                this.lastTapStartTime = Time.time;
            }

            this.UpdateControl();
            
            if (this.isDoubleTapping && !this.isTapping)
            {
                this.isDoubleTapping = false;
                this.OnStopDoubleTap();
            }
        }
        

        // Trails
        {
            float trailDuration;
            var aimDir = this.transform.right;
            var velDir = (Vector3)this.rigidBody.velocity.normalized;
            var dirDot = (Vector2.Dot(aimDir, velDir) + 1f) / 2f;

            if (this.isThrusting && dirDot > this.dotProductMinBeforeTrails)
            {
                trailDuration = this.baseTrailDuration * this.SpeedPercentage;
            }
            else
            {
                trailDuration = 0f;
            }

            foreach (var t in this.trails)
            {
                t.time = trailDuration;
            }
        }

        this.wasTapping = this.isTapping;
        this.wasThrusting = this.isThrusting;
    }


    #endregion
    
    
    #region Public
    

    public void AddImpulseForce(Vector2 force, bool overdrive = false)
    {
        if (!this.isRunning)
        {
            return;
        }

        this.rigidBody.AddForce(force, ForceMode2D.Impulse);
        
        if (overdrive)
        {
            this.currMaxSpeed = this.maxOverdriveSpeed;
        }

        if (this.rigidBody.velocity.magnitude > this.currMaxSpeed)
        {
            this.rigidBody.velocity = this.rigidBody.velocity.normalized * this.currMaxSpeed;
        }
    }


    #endregion


    #region Private


    private void UpdateControl()
    {
        var aimDir = this.transform.right;
        var velDir = (Vector3)this.rigidBody.velocity.normalized;
        var dirDot = (Vector2.Dot(aimDir, velDir) + 1f) / 2f;
    
        this.UpdateMaxSpeed();
            
        if (this.tapType == TapType.Both)
        {
            this.UpdateThrust(velDir, dirDot);

            if (!this.IsThrusting)
            {
                this.OnStartThrusting();
            }
        }
        else
        {
            if (this.tapType == TapType.Left)
            {
                this.UpdateSpin(true); 
                this.isCCW = true;
            }
            else if (this.tapType == TapType.Right)
            {
                this.UpdateSpin(false); 
                this.isCCW = false;
            }

            if (this.IsThrusting)
            {
                this.OnStopThrusting();
            }
        }
    }

    private void UpdateMaxSpeed()
    {
        // Decay max speed
        if (this.currMaxSpeed > this.maxSpeed)
        {
            var decay = this.maxSpeedDecayRate * Time.fixedDeltaTime;
            this.currMaxSpeed = Mathf.Clamp(this.currMaxSpeed - decay, this.maxSpeed, this.maxOverdriveSpeed);
        }
    }

    private void UpdateSpin(bool counterclockwise)
    {
        var direction = counterclockwise ? 1 : -1;

        // Add angular velocity
        var curveValue = this.timeReleased / this.torqueCurveDuration;
        var multiplier = this.torqueCurve.Evaluate(curveValue);
        this.rigidBody.AddTorque(this.torque * multiplier * direction);

        // Cap angular velocity
        if (Mathf.Abs(this.rigidBody.angularVelocity) > this.maxAngularVelocity)
        {
            this.rigidBody.angularVelocity = this.maxAngularVelocity * direction;
        }
    }
    
    private void UpdateThrust(Vector2 velDir, float dirDot)
    {
        // Decay previous linear velocity
        if (!this.wasThrusting)
        {
            var curveValue = this.velocityDecayCurve.Evaluate(dirDot);
            var multiplier = Mathf.Lerp(this.velocityDecayRange.x, this.velocityDecayRange.y, curveValue);
            
            var offsetCurveValue = this.velocityDecayCurveSpeedInfluence.Evaluate(this.SpeedPercentage);
            var offsetMultiplier = Mathf.Lerp(this.velocityDecaySpeedInfluenceRange.x, this.velocityDecaySpeedInfluenceRange.y, offsetCurveValue);
            
            var speed = Mathf.Clamp(this.rigidBody.velocity.magnitude * multiplier * offsetMultiplier, 0f, this.currMaxSpeed);
            this.rigidBody.velocity = velDir * speed;
        }

        // Add new linear velocity
        {
            var curveValue = this.boostMultiplierCurve.Evaluate(dirDot);
            var multiplier = Mathf.Lerp(this.boostMultiplierRange.x, this.boostMultiplierRange.y, curveValue);
            var timeCurveValue = this.boostTimeMultiplierCurve.Evaluate(this.timeReleased / this.boostTimeMultiplierDuration);
            var timeMultiplier = Mathf.Lerp(this.boostTimeMultiplierRange.x, this.boostTimeMultiplierRange.y, timeCurveValue);
            var force = this.InOverdrive ? this.boostOverdriveForce : this.boostForce;
            this.rigidBody.AddRelativeForce(force * multiplier * timeMultiplier);
        }

        // Cap linear velocity
        if (this.rigidBody.velocity.magnitude > this.currMaxSpeed)
        {
            var unitVel = this.rigidBody.velocity.normalized;
            this.rigidBody.velocity = unitVel * this.currMaxSpeed;
        }

        // Decay angular velocity
        {
            var curveValue = this.angularVelocityDecayCurve.Evaluate(timeHeld / this.angularVelocityDecayDuration);
            var multiplier = Mathf.Lerp(this.angularVelocityDecayRange.x, this.angularVelocityDecayRange.y, curveValue);
            this.rigidBody.angularVelocity *= multiplier;
        }
    }

    private void OnStartThrusting()
    {
        this.isThrusting = true;

//        this.beamParent.SetActive(true);
    }

    private void OnStopThrusting()
    {
        this.isThrusting = false;

//        this.beamParent.SetActive(false);
    }

    private void OnStartDoubleTap()
    {
        switch (this.doubleTapMode)
        {
            case DoubleTapMode.Shield: this.shieldController.Show(true); break;
        }
    }

    private void OnStopDoubleTap()
    {
        switch (this.doubleTapMode)
        {
            case DoubleTapMode.Shield: this.shieldController.Show(false); break;
        }
    }
    
    private TapType GetCurrentTapType()
    {
        if (GameManager.Instance.IsUsingKeyboard)
        {
            return Input.anyKey ? TapType.Both : TapType.Left;
        }

        switch (Input.touchCount)
        {
            case 0: return TapType.Left;
            default: return TapType.Both;
        }
    }


    #endregion


    private void HandleLevelStop()
    {
        this.isRunning = false;

        this.transform.position = this.initialPos;
        this.transform.rotation = this.initialRot;

        this.rigidBody.angularVelocity = 0f;
        this.rigidBody.velocity = Vector2.zero;

        foreach (var t in this.trails)
        {
            t.enabled = false;
        }

        this.currMaxSpeed = this.maxSpeed;
    }

    private void HandleLevelStart()
    {
        if (this.isRunning)
        {
            return;
        }

        this.isRunning = true;

        CameraController.Shake(this.launchShakeDuration, this.launchShakeMagnitude);

        this.AddImpulseForce(this.launchForce);

        foreach (var t in this.trails)
        {
            t.enabled = true;
        }
    }
}
