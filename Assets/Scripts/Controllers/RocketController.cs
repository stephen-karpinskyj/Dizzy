using System.Collections;
using UnityEngine;

public class RocketController : MonoBehaviour
{
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


    #region Inspector


    [Header("Thrust")]

    [SerializeField]
    private float maxSpeed = 7f;
    [SerializeField]
    private AnimationCurve boostMultiplierCurve;
    [SerializeField]
    private Vector2 boostForce = new Vector2(300f, 0f);
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
    private Vector2 rotationXRange = new Vector2(-80f, 0f);
    [SerializeField]
    private float rotationXSpeed = 200f;
    [SerializeField]
    private float rotationXStartTime = 0.05f;

    [SerializeField]
    private AnimationCurve barrelRollCurve;
    [SerializeField]
    private float barrelRollDelay = 0.15f;
    [SerializeField]
    private float barrelRollDuration = 1f;


    [Header("Other")]

    [SerializeField]
    private ControlMode controlMode = ControlMode.SpinCCW;

    [SerializeField]
    private Rigidbody2D rigidBody;

    [SerializeField]
    private Transform bodyTransform;

    [SerializeField]
    private AudioSource source;

    [SerializeField]
    private TrailRenderer[] trails;

    [SerializeField]
    private float baseTrailDuration = 0.1f;

    [SerializeField]
    private float dotProductMinBeforeTrails = 0.9f;

    [SerializeField]
    private float controlDelay = 0.5f;

    [SerializeField]
    private float doubleTapDuration = 0.2f;

    [SerializeField]
    private float minTimeBetweenDoubleTaps = 0.2f;

    [SerializeField]
    private float pitchIncreaseMultiplier = 0.3f;


    #endregion


    private bool inWinMode = false;
    private float timeHeld = 0f;
    private float timeReleased = 0f;
    private bool isTapping = false;
    private bool wasTapping = false;
    private float timeThrusting = 0f;
    private bool isThrusting = false;
    private bool wasThrusting = false;

    private float currentBarrelRoll;
    private float rotXOnInputChange;

    private float controlTime;

    private float angularVelocityOnRelease = 0f;

    private bool isCCW = true;

    private DebugLine debugBlueLine;
    private DebugLine debugYellowLine;
    private DebugText debugText;
    //private string debugString;

    public Vector3 Position
    {
        get { return this.transform.position; }
    }

    public float SpeedPercentage
    {
        get { return this.rigidBody.velocity.magnitude / this.maxSpeed; }
    }

    private bool HasControl
    {
        get { return Time.time < this.controlTime; }
    }

    public bool IsThrusting
    { 
        get { return this.isThrusting; }
    }

    public bool IsCCW
    {
        get { return this.isCCW; }
    }

    private static RocketController instance;

    public static RocketController Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        RocketController.instance = this;
        
        Debug.Assert(this.rigidBody != null, this);
        Debug.Assert(this.source != null, this);

        Broadcast.SendMessage("TimeStart");

        this.debugBlueLine = DebugLine.Draw(Vector3.zero, Vector3.zero, Color.blue);
        this.debugYellowLine = DebugLine.Draw(Vector3.zero, Vector3.zero, Color.yellow);
        this.debugText = DebugText.Draw(Vector3.zero, string.Empty);

        //this.StartCoroutine(this.RollCoroutine());
    }

    private void OnDestroy()
    {
        if (!this.inWinMode)
        {
            Broadcast.SendMessage("LevelStart");
        }

        if (this.debugBlueLine)
        {
            Object.Destroy(this.debugBlueLine.gameObject);
        }

        if (this.debugYellowLine)
        {
            Object.Destroy(this.debugYellowLine.gameObject);
        }

        if (this.debugText)
        {
            Object.Destroy(this.debugText);
        }
    }

    private void Update()
    {
        this.isTapping = Input.anyKey || this.HasControl;

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

        // Audio
        {
            this.source.pitch = 1f + this.timeThrusting * this.pitchIncreaseMultiplier;

            if (this.wasThrusting && !this.isThrusting)
            {
                this.source.Stop();
            }
            else if (!this.wasThrusting && this.isThrusting)
            {
                this.source.Play();
            }
        }

        this.wasTapping = this.isTapping;
        this.wasThrusting = this.isThrusting;

        //this.debugString = string.Format("{0:f2}", this.currentFullRoll);
        //this.debugString = string.Empty;

        this.debugText.Move(this.transform.position + Vector3.up * 1f);
        //this.debugText.Text = this.debugString;
    }

    private void LevelWin()
    {
        this.inWinMode = true;
        Object.Destroy(this.gameObject);
    }

    private void LevelStart()
    {
        this.inWinMode = false;
        Object.Destroy(this.gameObject);
    }

    private void TimeStart()
    {
        this.controlTime = Time.time + this.controlDelay;
    }

    public void AddForce(Vector2 force)
    {
        this.rigidBody.AddForce(force, ForceMode2D.Impulse);

        if (this.rigidBody.velocity.magnitude > this.maxSpeed)
        {
            this.rigidBody.velocity = this.rigidBody.velocity.normalized * this.maxSpeed;
        }
    }

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

    private void HandleControl()
    {
        switch (this.controlMode)
        {
            case ControlMode.SpinCCW: this.isCCW = true; this.ControlSpin(true); break;
            case ControlMode.SpinCW: this.isCCW = false; this.ControlSpin(false); break;
            case ControlMode.GoToPoint: this.ControlGoToPoint(); break;
            case ControlMode.DualSpin: this.ControlDualSpin(); break;
        }
    }

    private void ControlSpin(bool counterclockwise)
    {
        if (this.isTapping)
        {
            var aimDir = this.transform.right;
            var velDir = (Vector3)this.rigidBody.velocity.normalized;
            var dirDot = (Vector2.Dot(aimDir, velDir) + 1f) / 2f;

            // Decay previous linear velocity
            if (Input.anyKeyDown)
            {
                var curveValue = this.velocityDecayCurve.Evaluate(dirDot);
                var multiplier = Mathf.Lerp(this.velocityDecayRange.x, this.velocityDecayRange.y, curveValue);
                this.rigidBody.velocity = velDir * this.rigidBody.velocity.magnitude * multiplier;
            }

            // Add new linear velocity
            {
                var curveValue = this.boostMultiplierCurve.Evaluate(dirDot);
                var multiplier = Mathf.Lerp(this.boostMultiplierRange.x, this.boostMultiplierRange.y, curveValue);
                var timeCurveValue = this.boostTimeMultiplierCurve.Evaluate(this.timeReleased / this.boostTimeMultiplierDuration);
                var timeMultiplier = Mathf.Lerp(this.boostTimeMultiplierRange.x, this.boostTimeMultiplierRange.y, timeCurveValue);
                this.rigidBody.AddRelativeForce(this.boostForce * multiplier * timeMultiplier);
            }

            // Cap linear velocity
            if (this.rigidBody.velocity.magnitude > this.maxSpeed)
            {
                var unitVel = this.rigidBody.velocity.normalized;
                this.rigidBody.velocity = unitVel * this.maxSpeed;
            }

            // Decay angular velocity
            {
                var curveValue = this.angularVelocityDecayCurve.Evaluate(timeHeld / this.angularVelocityDecayDuration);
                var multiplier = Mathf.Lerp(this.angularVelocityDecayRange.x, this.angularVelocityDecayRange.y, curveValue);
                this.rigidBody.angularVelocity *= multiplier * this.SpeedPercentage;
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
            if (this.isTapping && this.timeHeld >= rotationXStartTime)
            {
                target = this.rotationXRange.x;
            }
            else if (this.isCCW)
            {
                target = this.rotationXRange.y;
            }
            else
            {
                target = -this.rotationXRange.y;
            }

            if (Mathf.Abs(this.xRot - target) > 0.1f)
            {
                var dir = Mathf.Sign(target - this.xRot);
                this.xRot += dir * this.rotationXSpeed * Time.deltaTime;
                var bodyEuler = new Vector3(this.xRot + this.currentBarrelRoll, -180f, -180f);
                this.bodyTransform.localRotation = Quaternion.Euler(bodyEuler);
            }
        }
    }

    private float xRot = 0f;

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
                if (this.rigidBody.velocity.magnitude > this.maxSpeed)
                {
                    this.rigidBody.velocity = this.rigidBody.velocity.normalized * this.maxSpeed;
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


        //this.debugString = string.Format("{0:f2}/{1:f2} ({2:f2})", this.rigidBody.angularVelocity, maxAngVel, newAngVel);
        //this.debugString = string.Format("{0:f2}/{1:f2} ({2:f2})", this.rigidBody.velocity.magnitude, this.maxSpeed, newForce.magnitude);
        //this.debugString = string.Format("{0:f2}", dirDot);
        //this.debugBlueLine.Move(this.transform.position, this.transform.position + velDir * 1f);
        //this.debugYellowLine.Move(this.transform.position, this.transform.position + inputDir * 1f);
    }

    private float lastTapStartTime;
    private float lastDoubleTapTime;
    private void ControlDualSpin()
    {
        var canDoubleTap = (Time.time - this.lastDoubleTapTime) >= this.minTimeBetweenDoubleTaps;
        if (canDoubleTap && this.isTapping && !this.wasTapping)
        {
            var isDoubleTapping = (Time.time - this.lastTapStartTime) <= this.doubleTapDuration;
            if (isDoubleTapping)
            {
                this.isCCW = !this.isCCW;
                this.lastDoubleTapTime = Time.time;
            }

            this.lastTapStartTime = Time.time;
        }

        this.ControlSpin(this.isCCW);
    }
}
