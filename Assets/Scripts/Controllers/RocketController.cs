using UnityEngine;

public class RocketController : MonoBehaviour
{
    [Header("Boost")]

    [SerializeField]
    private Vector2 boostForce = new Vector2(300f, 0f);

    [SerializeField]
    private float maxSpeed = 7f;

    [SerializeField]
    private AnimationCurve boostMultiplierCurve;

    [SerializeField]
    private Vector2 boostMultiplierRange = new Vector2(0f, 1f);

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
    private float torque = 150f;

    [SerializeField]
    private AnimationCurve torqueCurve;

    [SerializeField]
    private float torqueCurveDuration = 0.5f;

    [SerializeField]
    private float maxAngularVelocity = 260f;


    [Header("Other")]

    [SerializeField]
    private Rigidbody2D rigidBody;

    [SerializeField]
    private AudioSource source;

    [SerializeField]
    private float pitchIncreaseMultiplier = 0.3f;

    private bool inWinMode = false;
    private float timeHeld = 0f;
    private float timeReleased = 0f;

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

        if (Input.anyKey)
        {
            this.timeReleased = 0f;
            this.timeHeld += Time.deltaTime;

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
                this.rigidBody.AddRelativeForce(this.boostForce * multiplier);
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
            this.source.Play();
        }
        else
        {
            this.timeHeld = 0f;
            this.timeReleased += Time.deltaTime;

            // Add angular velocity
            var curveMultiplier = this.torqueCurve.Evaluate(this.timeReleased / this.torqueCurveDuration);
            this.rigidBody.AddTorque(this.torque * curveMultiplier);

            // Cap angular velocity
            if (this.rigidBody.angularVelocity > this.maxAngularVelocity)
            {
                this.rigidBody.angularVelocity = this.maxAngularVelocity;
            }

            this.source.Pause();
        }

        //this.debugDiffString = string.Format("{0:f2}", this.rigidBody.angularVelocity);
        //this.debugDiffString = string.Empty;

        //this.debugAimLine.Move(this.transform.position, this.transform.position + aimDir * 0.45f);
        //this.debugVelocityLine.Move(this.transform.position, this.transform.position + velDir * 0.45f);
        //this.debugDiffText.Move(this.transform.position + Vector3.up * 1f);
        //this.debugDiffText.Text = this.debugDiffString;
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
}
