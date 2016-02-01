using System.Collections;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    #region Types


    private enum ControlMode
    {
        /// <summary>
        /// Tapping: thrust.
        /// None: drift rotating in last direction.
        /// </summary>
        SpinCCW = 0,

        /// <summary>
        /// Tapping: thrust.
        /// None: drift rotating in last direction.
        /// </summary>
        SpinCW,

        /// <summary>
        /// Tapping: rotate/thrust toward point.
        /// None: drift without rotation.
        /// </summary>
        GoToPoint,

        /// <summary>
        /// Tapping left: thrust/change direction to ccw.
        /// Tapping right: thrust/change direction to cw.
        /// None: drift rotating in last direction.
        /// </summary>
        DualSpin,
    }

    private enum DoubleTapMode
    {
        Nothing = 0,

        Shield,
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


    [Header("Aim")]

    [SerializeField]
    private AnimationCurve aimMaxAngularVelocityCurve;
    [SerializeField]
    private Vector2 aimMaxAngularVelocityRange = new Vector2(0f, 5f);
    [SerializeField]
    private float aimTorque = 300f;
    [SerializeField]
    private AnimationCurve aimVelocityDecayCurve;
    [SerializeField]
    private Vector2 aimVelocityDecayRange = new Vector2(0.8f, 0.9f);
    [SerializeField]
    private AnimationCurve aimBoostMultiplierCurve;
    [SerializeField]
    private Vector2 aimBoostMultiplierRange = new Vector2(0.8f, 1f);
    [SerializeField]
    private float dotProductLimitBeforeThrust = 0.01f;


    [Header("Roll")]

    [SerializeField]
    private float minRotationXSpeed = 100f;
    [SerializeField]
    private float maxRotationXSpeed = 200;

    [SerializeField]
    private AnimationCurve barrelRollCurve;
    [SerializeField]
    private float barrelRollDelay = 0.15f;
    [SerializeField]
    private float barrelRollDuration = 1f;

    [SerializeField]
    private float maxRollAimVelDiff = 10f;

    [SerializeField]
    private Vector2 easeRollSpeedMultiplierRange = new Vector2(0.3f, 1f);
    [SerializeField]
    private float easeRollSpeedMultiplierDuration = 1f;
    
    [SerializeField]
    private Vector2 thrustRollSpeedMultiplierRange = new Vector2(1f, 2.5f);
    [SerializeField]
    private float thrustRollSpeedMultiplierDuration = 0.5f;
    
    [SerializeField]
    private float rollSpeedLinearSpeedFactor = 1.2f;


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
    private ControlMode controlMode = ControlMode.SpinCCW;

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

    private float timeHeld = 0f;
    private float timeReleased = 0f;
    private bool isTapping = false;
    private bool wasTapping = false;
    private float timeThrusting = 0f;
    private bool isThrusting = false;
    private bool wasThrusting = false;
    private bool isDoubleTapping = false;

    private float currentBarrelRoll;

    private float angularVelocityOnRelease = 0f;

    private bool isRunning = false;
    private bool isCCW = false;

    private float xRot = 0f;

    private float lastTapStartTime;
    private float lastDoubleTapTime;
    
    private float currMaxSpeed;

    //private DebugLine debugBlueLine;
    //private DebugLine debugYellowLine;
    //private DebugText debugText;
    //private string debugString;


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
        Debug.Assert(this.rigidBody != null, this);

        //this.debugBlueLine = DebugLine.Draw(Vector3.zero, Vector3.zero, Color.blue);
        //this.debugYellowLine = DebugLine.Draw(Vector3.zero, Vector3.zero, Color.yellow);
        //this.debugText = DebugText.Draw(Vector3.zero, string.Empty);

        this.StartCoroutine(this.RollCoroutine());

        this.initialPos = this.transform.position;
        this.initialRot = this.transform.rotation;

        this.currMaxSpeed = this.maxSpeed;

        this.OnLevelStop();
    }

    private void Update()
    {
        if (!this.isRunning)
        {
            return;
        }
        
        this.isTapping = Input.anyKey;

        if (this.isTapping)
        {
            this.timeReleased = 0f;
            this.timeHeld += Time.deltaTime;
        }
        else
        {
            this.timeHeld = 0f;
            this.timeReleased += Time.deltaTime;
        }

        if (this.isThrusting)
        {
            this.timeThrusting += Time.deltaTime;
        }
        else
        {
            this.timeThrusting = 0f;
        }

        this.HandleControl();

        // Trails
        {
            var trailDuration = 0f;
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

        //this.debugString = string.Format("{0:f2}", this.currentFullRoll);
        //this.debugString = string.Empty;

        //this.debugText.Move(this.transform.position + Vector3.up * 1f);
        //this.debugText.Text = this.debugString;
    }


    #endregion


    #region Level events


    public void OnLevelStop()
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

    public void OnLevelStart()
    {
        if (this.isRunning)
        {
            return;
        }

        this.isRunning = true;
        this.isCCW = StateManager.Instance.SpinDirectionCCW;

        CameraController.Shake(this.launchShakeDuration, this.launchShakeMagnitude);

        this.AddImpulseForce(this.launchForce);

        foreach (var t in this.trails)
        {
            t.enabled = true;
        }
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


    #region Coroutines


    private IEnumerator RollCoroutine()
    {
        yield return new WaitForSeconds(this.barrelRollDelay);

        if (!this.isThrusting)
        {
            yield break;
        }

        var startTime = Time.time;
        var timeSoFar = 0f;
        var t = 0f;

        do
        {
            timeSoFar = Time.time - startTime;
            t = timeSoFar / this.barrelRollDuration;

            var curveValue = this.barrelRollCurve.Evaluate(t);
            this.currentBarrelRoll = Mathf.Lerp(0, -360f, curveValue);

            yield return null;

        }
        while (t < 1f);
    }


    #endregion


    #region Private


    private void HandleControl()
    {
        switch (this.controlMode)
        {
            case ControlMode.SpinCCW: this.isCCW = true; this.ControlSpin(true); break;
            case ControlMode.SpinCW: this.isCCW = false; this.ControlSpin(false); break;
            case ControlMode.GoToPoint: this.ControlGoToPoint(); break;
            case ControlMode.DualSpin: this.ControlWithDoubleTap(); break;
        }
    }

    private void ControlSpin(bool counterclockwise)
    {
        var aimDir = this.transform.right;
        var velDir = (Vector3)this.rigidBody.velocity.normalized;
        var dirDot = (Vector2.Dot(aimDir, velDir) + 1f) / 2f;
    
        // Decay max speed
        if (this.currMaxSpeed > this.maxSpeed)
        {
            var decay = this.maxSpeedDecayRate * Time.smoothDeltaTime;
            this.currMaxSpeed = Mathf.Clamp(this.currMaxSpeed - decay, this.maxSpeed, this.maxOverdriveSpeed);
        }
            
        if (this.isTapping)
        {
            // Decay previous linear velocity
            if (Input.anyKeyDown)
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

            this.isThrusting = true;
        }
        else
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

            this.isThrusting = false;
        }

        // Handle x-rotation
        {
            var target = 0f;
              
            var speedFactor = (1f - dirDot) * this.SpeedPercentage * this.rollSpeedLinearSpeedFactor;  
            var velAimDiffRot = this.maxRollAimVelDiff * this.AngularSpeedPercentage * speedFactor;
            
            if (this.isCCW)
            {
                target += velAimDiffRot;
            }
            else
            {
                target -= velAimDiffRot;
            }
        
            if (Mathf.Abs(this.xRot - target) > 0.2f)
            {
                var speed = Mathf.Lerp(this.minRotationXSpeed, this.maxRotationXSpeed, this.SpeedPercentage);
                
                if (this.IsThrusting)
                {
                    speed *= Mathf.Lerp(this.thrustRollSpeedMultiplierRange.x, this.thrustRollSpeedMultiplierRange.y, this.timeThrusting / this.thrustRollSpeedMultiplierDuration);
                }
                else
                {
                    speed *= Mathf.Lerp(this.easeRollSpeedMultiplierRange.x, this.easeRollSpeedMultiplierRange.y, this.timeReleased / this.easeRollSpeedMultiplierDuration);
                }
                
                var dir = Mathf.Sign(target - this.xRot);
                this.xRot += dir * speed * Time.smoothDeltaTime;
                var bodyEuler = new Vector3(this.xRot, -180f, -180f);
                this.bodyTransform.localRotation = Quaternion.Euler(bodyEuler);
            }
        }

        //this.debugString = string.Format("{0:f2}", );
        
        //this.debugBlueLine.Move(this.transform.position, this.transform.position + aimDir * 1f);
        //this.debugYellowLine.Move(this.transform.position, this.transform.position + velDir * 1f);
    }
    
    private void ControlGoToPoint()
    {
        var worldInputPos = (Vector3)(Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition); // clear z
        var worldPos = (Vector3)(Vector2)this.transform.position; // clear z
        var inputDir = (worldInputPos - worldPos).normalized;
        var aimDir = this.transform.right;
        var velDir = (Vector3)this.rigidBody.velocity.normalized;
        var dirDot = (Vector2.Dot(aimDir, velDir) + 1f) / 2f;
        var inputInvDot = 1f - ((Vector2.Dot(aimDir, inputDir) + 1f) / 2f);
        var angleDiff = aimDir.SignedAngle(inputDir);
        var normAngleDiff = angleDiff / 180f;

        this.isThrusting = false;

        if (Input.GetMouseButton(0))
        {
            // Torque
            {
                // Add
                var newAngVel = normAngleDiff * this.aimTorque;
                this.rigidBody.AddTorque(newAngVel, ForceMode2D.Force);

                // Cap
                {
                    var maxAngVelDir = Mathf.Sign(angleDiff);
                    var maxAngVel = Mathf.Lerp(this.aimMaxAngularVelocityRange.x, this.aimMaxAngularVelocityRange.y, this.aimMaxAngularVelocityCurve.Evaluate(inputInvDot)) * maxAngVelDir;

                    if (Mathf.Abs(this.rigidBody.angularVelocity) > Mathf.Abs(maxAngVel))
                    {
                        this.rigidBody.angularVelocity = maxAngVel;
                    }
                }
            }

            // Thrust
            if (inputInvDot < this.dotProductLimitBeforeThrust)
            {
                this.isThrusting = true;

                // Decay
                if (!this.wasThrusting)
                {
                    var multiplier = Mathf.Lerp(this.aimVelocityDecayRange.x, this.aimVelocityDecayRange.y, this.aimVelocityDecayCurve.Evaluate(dirDot));
                    this.rigidBody.velocity = velDir * this.rigidBody.velocity.magnitude * multiplier;
                }

                // Add
                {
                    var multiplier = Mathf.Lerp(this.aimBoostMultiplierRange.x, this.aimBoostMultiplierRange.y, this.aimBoostMultiplierCurve.Evaluate(dirDot));
                    this.rigidBody.AddRelativeForce(this.boostForce * multiplier);
                }

                // Cap
                if (this.rigidBody.velocity.magnitude > this.currMaxSpeed)
                {
                    this.rigidBody.velocity = this.rigidBody.velocity.normalized * this.currMaxSpeed;
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonUp(0))
            {
                this.angularVelocityOnRelease = this.rigidBody.angularVelocity;
            }

            this.rigidBody.angularVelocity = this.angularVelocityOnRelease;

            // Cap
            if (Mathf.Abs(this.rigidBody.angularVelocity) > this.maxAngularVelocity)
            {
                this.rigidBody.angularVelocity = Mathf.Sign(this.rigidBody.angularVelocity) * this.maxAngularVelocity;
            }
        }

        // Handle x-rotation
        {
            //var angVelPer = this.rotationXCurve.Evaluate(this.rigidBody.angularVelocity / this.maxAngularVelocity);
            //var xRot = Mathf.Lerp(this.rotationXRange.x / 2f, this.rotationXRange.y / 2f, Mathf.Abs(angVelPer));
            //xRot *= Mathf.Sign(angVelPer);

            var bodyEuler = new Vector3(/*xRot + */this.currentBarrelRoll, -180f, -180f);
            this.bodyTransform.localRotation = Quaternion.Euler(bodyEuler);
        }
    }

    private void ControlWithDoubleTap()
    {
        var canDoubleTap = (Time.time - this.lastDoubleTapTime) >= this.minTimeBetweenDoubleTaps;
        if (canDoubleTap && this.isTapping && !this.wasTapping)
        {
            var doubleTapped = (Time.time - this.lastTapStartTime) <= this.doubleTapDuration;
            if (doubleTapped)
            {
                // TODO: Do special action with double tap
                this.lastDoubleTapTime = Time.time;
                this.isDoubleTapping = true;
                this.OnStartDoubleTap();
            }

            this.lastTapStartTime = Time.time;
        }

        this.ControlSpin(this.isCCW);

        if (this.isDoubleTapping && !this.isTapping)
        {
            this.isDoubleTapping = false;
            this.OnStopDoubleTap();
        }
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


    #endregion
}
