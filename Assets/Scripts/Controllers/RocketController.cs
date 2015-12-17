using UnityEngine;

public class RocketController : MonoBehaviour
{
    [SerializeField]
    private Vector2 boostForce = new Vector2(280f, 0f);

    [SerializeField]
    private Vector2 idleForce = new Vector2(0f, 0f);

    [SerializeField]
    private float torque = 150f;

    [SerializeField]
    private float maxAngularVelocity = 260f;

    [SerializeField]
    private float angularVelocityDecayOnPress = 0.2f;

    [SerializeField]
    private float velocityDecayOnPress = 0.3f;

    [SerializeField]
    private Rigidbody2D rigidBody;

    [SerializeField]
    private AudioSource source;

    [SerializeField]
    private float pitchIncreaseMultiplier = 0.3f;

    private bool inWinMode = false;
    private float timeHeld = 0;

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
    }

    private void OnDestroy()
    {
        Count--;

        if (Count == 0 && !this.inWinMode)
        {
            Broadcast.SendMessage("LevelStart");
        }
    }

    private void Update()
    {
        if (Input.anyKey)
        {
            this.timeHeld += Time.deltaTime;
            this.source.pitch = 1f + this.timeHeld * this.pitchIncreaseMultiplier;

            if (Input.anyKeyDown)
            {
                this.rigidBody.velocity *= this.velocityDecayOnPress;
                this.source.Play();
            }

            this.rigidBody.AddRelativeForce(this.boostForce);
            this.rigidBody.angularVelocity *= this.angularVelocityDecayOnPress;
        }
        else
        {
            this.rigidBody.AddRelativeForce(this.idleForce);
            this.rigidBody.AddTorque(this.torque);

            if (this.rigidBody.angularVelocity > this.maxAngularVelocity)
            {
                this.rigidBody.angularVelocity = this.maxAngularVelocity;
            }

            this.timeHeld = 0;

            this.source.Pause();
        }
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
