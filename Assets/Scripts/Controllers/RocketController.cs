using UnityEngine;

public class RocketController : MonoBehaviour
{
    [Header("Boost")]

    [SerializeField]
    private Vector2 boostForce = new Vector2(300f, 0f);

    [SerializeField]
    private float maxVelocity = 500f;

    [SerializeField]
    private Vector2 velocityDecayRange = new Vector2(1f, 0.35f);

    [SerializeField]
    private AnimationCurve boostMultiplierCurve;

    [SerializeField]
    private Vector2 boostMultiplierRange = new Vector2(0f, 1f);


    [Header("Spin")]

    [SerializeField]
    private float torque = 150f;

    [SerializeField]
    private float maxAngularVelocity = 260f;

    [SerializeField]
    private float angularVelocityDecayOnPress = 0.2f;


    [Header("Other")]

    [SerializeField]
    private Rigidbody2D rigidBody;

    [SerializeField]
    private AudioSource source;

    [SerializeField]
    private float pitchIncreaseMultiplier = 0.3f;

    private bool inWinMode = false;
    private float timeHeld = 0;

    private DebugLine debugAimLine;
    private DebugLine debugVelocityLine;
    private DebugText debugDiffText;
    private string debugDiffString;

    private static int Count = 0;

    public static int CurrentCount
    {
        get { return Count; }
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
        var dirDot = Mathf.Abs(Vector2.Dot(aimDir, velDir));

        if (Input.anyKey)
        {
            this.timeHeld += Time.deltaTime;
            this.source.pitch = 1f + this.timeHeld * this.pitchIncreaseMultiplier;

            if (Input.anyKeyDown)
            {
                var decayMultiplier = Mathf.Lerp(this.velocityDecayRange.x, this.velocityDecayRange.y, 1f - dirDot);

                var decayedSpeed = this.rigidBody.velocity.magnitude * decayMultiplier;

                this.rigidBody.velocity = velDir * decayedSpeed;

                this.source.Play();
            }

            var boostMultiplier = Mathf.Lerp(this.boostMultiplierRange.x, this.boostMultiplierRange.y, this.boostMultiplierCurve.Evaluate(dirDot)); //this.boostCurve.Evaluate(boostCurveTime);
            this.rigidBody.AddRelativeForce(this.boostForce * boostMultiplier);
            this.rigidBody.angularVelocity *= this.angularVelocityDecayOnPress;

            if (this.rigidBody.velocity.magnitude > this.maxVelocity)
            {
                var unitVel = this.rigidBody.velocity.normalized;
                this.rigidBody.velocity = unitVel * this.maxVelocity;
            }

            //this.debugDiffString = string.Format("{0:f2}", boostMultiplier);
        }
        else
        {
            this.rigidBody.AddTorque(this.torque);

            if (this.rigidBody.angularVelocity > this.maxAngularVelocity)
            {
                this.rigidBody.angularVelocity = this.maxAngularVelocity;
            }

            this.timeHeld = 0;

            this.source.Pause();

            this.debugDiffString = string.Empty;
        }

        this.debugAimLine.Move(this.transform.position, this.transform.position + aimDir * 0.38f);
        this.debugVelocityLine.Move(this.transform.position, this.transform.position + velDir * 0.38f);
        this.debugDiffText.Move(this.transform.position + Vector3.up * 1f);
        this.debugDiffText.Text = this.debugDiffString;
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
