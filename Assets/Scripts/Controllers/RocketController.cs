using UnityEngine;
using System.Collections;

public class RocketController : MonoBehaviour
{
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


    [Header("Yaw")]

    [SerializeField]
    private float maxAngularVelocity = 260f;
    [SerializeField]
    private AnimationCurve torqueCurve;
    [SerializeField]
    private float torque = 150f;
    [SerializeField]
    private float torqueCurveDuration = 0.5f;


    [Header("Roll")]

    [SerializeField]
    private Vector2 rotationXRange = new Vector2(-80f, 0f);
    [SerializeField]
    private AnimationCurve rotationXCurve;
    [SerializeField]
    private float rotationXStartTime = 0.02f;
    [SerializeField]
    private float rotationXStartDuration = 0.1f;
    [SerializeField]
    private float rotationXEndTime = 0.02f;
    [SerializeField]
    private float rotationXEndDuration = 0.1f;

    [SerializeField]
    private AnimationCurve barrelRollCurve;
    [SerializeField]
    private float barrelRollDelay = 0.15f;
    [SerializeField]
    private float barrelRollDuration = 1f;


    [Header("Other")]

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
    private float pitchIncreaseMultiplier = 0.3f;


    #endregion


    private bool inWinMode = false;
    private float timeHeld = 0f;
    private float timeReleased = 0f;
    private bool wasTapping = false;

    private float currentBarrelRoll;
    private float rotXOnInputChange;

    private DebugLine debugAimLine;
    private DebugLine debugVelocityLine;
    private DebugText debugDiffText;
    private string debugDiffString;

    private static int Count = 0;

    public static int CurrentCount
    {
        get { return Count; }
    }

    public float MaxSpeed
    {
        get { return this.maxSpeed; }
    }

    public Rigidbody2D MainRigidbody
    {
        get { return this.rigidBody; }
    }

    private void Awake()
    {
        Debug.Assert(this.rigidBody != null, this);
        Debug.Assert(this.source != null, this);

        Count++;

        if (Count == 1)
        {
            Broadcast.SendMessage("TimeStart");
        }

        this.debugAimLine = DebugLine.Draw(Vector3.zero, Vector3.zero, Color.blue);
        this.debugVelocityLine = DebugLine.Draw(Vector3.zero, Vector3.zero, Color.yellow);
        this.debugDiffText = DebugText.Draw(Vector3.zero, string.Empty, Color.blue);

        this.StartCoroutine(this.RollCoroutine());
    }

    private void OnDestroy()
    {
        Count--;

        if (Count == 0 && !this.inWinMode)
        {
            Broadcast.SendMessage("LevelStart");
        }

        if (this.debugAimLine)
        {
            Object.Destroy(this.debugAimLine.gameObject);
        }

        if (this.debugVelocityLine)
        {
            Object.Destroy(this.debugVelocityLine.gameObject);
        }

        if (this.debugDiffText)
        {
            Object.Destroy(this.debugDiffText);
        }
    }

    private void Update()
    {
        var aimDir = this.transform.right;
        var velDir = (Vector3)this.rigidBody.velocity.normalized;
        var dirDot = (Vector2.Dot(aimDir, velDir) + 1f) / 2f;

        var isTapping = Input.anyKey;

        if (isTapping)
        {
            this.timeReleased = 0f;
            this.timeHeld += Time.deltaTime;

            // Decay previous linear velocity
            if (Input.anyKeyDown)
            {
                var curveValue = this.velocityDecayCurve.Evaluate(dirDot);
                var multiplier = Mathf.Lerp(this.velocityDecayRange.x, this.velocityDecayRange.y, curveValue);
                this.rigidBody.velocity = velDir * this.rigidBody.velocity.magnitude * multiplier;

                this.source.Play();
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
                var speedMultiplier = this.rigidBody.velocity.magnitude / this.maxSpeed;
                this.rigidBody.angularVelocity *= multiplier * speedMultiplier;
            }

            this.source.pitch = 1f + this.timeHeld * this.pitchIncreaseMultiplier;
        }
        else
        {
            this.timeHeld = 0f;
            this.timeReleased += Time.deltaTime;

            // Add angular velocity
            var curveValue = this.timeReleased / this.torqueCurveDuration;
            var multiplier = this.torqueCurve.Evaluate(curveValue);
            this.rigidBody.AddTorque(this.torque * multiplier);

            // Cap angular velocity
            if (this.rigidBody.angularVelocity > this.maxAngularVelocity)
            {
                this.rigidBody.angularVelocity = this.maxAngularVelocity;
            }

            this.source.Pause();
        }

        // Handle x-rotation
        {
            var bodyEuler = this.bodyTransform.localEulerAngles;
            if (isTapping != wasTapping)
            {
                this.rotXOnInputChange = bodyEuler.x;
            }

            var curveValue = 0f;
            if (isTapping)
            {
                curveValue = 1f - (this.timeHeld - this.rotationXStartTime) / this.rotationXStartDuration;
            }
            else
            {
                curveValue = (this.timeReleased - rotationXEndTime) / this.rotationXEndDuration;
            }

            var t = this.rotationXCurve.Evaluate(curveValue);
            var min = isTapping ? this.rotationXRange.x : this.rotXOnInputChange;
            var max = isTapping ? this.rotXOnInputChange : this.rotationXRange.y;

            if (min > 180f)
            {
                min -= 360f;
            }

            if (max > 180f)
            {
                max -= 360f;
            }

            var angVelMultiplier = Mathf.Lerp(0f, 0.5f, this.rigidBody.angularVelocity / this.maxAngularVelocity);
            var multiplier = Mathf.Max(angVelMultiplier, t);
            var xRot = Mathf.Lerp(min, max, multiplier);

            bodyEuler = new Vector3(xRot + this.currentBarrelRoll, -180f, -180f);

            this.bodyTransform.localRotation = Quaternion.Euler(bodyEuler);
        }

        // Trails
        {
            var trailDuration = 0f;

            if (isTapping)
            {
                trailDuration = this.baseTrailDuration * (this.rigidBody.velocity.magnitude / this.maxSpeed);
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

        //this.debugDiffString = string.Format("{0:f2}", this.currentFullRoll);

        //this.debugAimLine.Move(this.transform.position, this.transform.position + aimDir * 0.45f);
        //this.debugVelocityLine.Move(this.transform.position, this.transform.position + velDir * 0.45f);
        this.debugDiffText.Move(this.transform.position + Vector3.up * 1f);
        this.debugDiffText.Text = this.debugDiffString;

        this.wasTapping = isTapping;
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

    public void Boost(Vector2 force)
    {
        this.rigidBody.AddForce(force, ForceMode2D.Impulse);
    }

    private IEnumerator RollCoroutine()
    {
        yield return new WaitForSeconds(this.barrelRollDelay);

        if (!Input.anyKey)
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
}
