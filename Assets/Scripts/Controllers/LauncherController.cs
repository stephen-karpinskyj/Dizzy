using System.Collections;
using UnityEngine;

public class LauncherController : MonoBehaviour
{
    [SerializeField]
    private RocketController rocketPrefab;

    [SerializeField]
    private Transform emitter;

    [SerializeField]
    private Vector2 initialForce = new Vector2(150, 0);

    [SerializeField]
    private float shakeDuration = 0.2f;

    [SerializeField]
    private float shakeMagnitude = 0.1f;

    [SerializeField]
    private float timeToWaitOnWin = 0.5f;

    [SerializeField]
    private AudioSource source;

    [SerializeField]
    private AudioClip launchClip;

    private bool needToRelease = false;
    private bool readyToGo = true;

    private bool wasAnyKey = false;

    private void Awake()
    {
        Debug.Assert(this.rocketPrefab, this);
        Debug.Assert(this.emitter, this);
        Debug.Assert(this.source, this);
        Debug.Assert(this.launchClip, this);
    }

    private void Update()
    {
        if (this.needToRelease)
        {
            if (this.wasAnyKey && !Input.anyKey)
            {
                this.needToRelease = false;
            }
            else
            {
                this.wasAnyKey = Input.anyKey;
                return;
            }
        }

        this.wasAnyKey = Input.anyKey;

        if (!this.readyToGo)
        {
            return;
        }

        if (Input.anyKey)
        {
            if (RocketController.CurrentCount == 0)
            {
                CameraController.Instance.Shake(this.shakeDuration, this.shakeMagnitude);

                var rocket = GameObjectUtility.InstantiatePrefab<RocketController>(this.rocketPrefab);
                rocket.transform.position = emitter.position;
                rocket.transform.rotation = emitter.rotation;
                rocket.Boost(this.initialForce);

                this.source.clip = this.launchClip;
                this.source.Play();
            }
        }
    }

    private void LevelWin()
    {
        this.StartCoroutine(this.BlockLaunch());

        if (Input.anyKey)
        {
            this.needToRelease = true;
        }
    }

    private IEnumerator BlockLaunch()
    {
        this.readyToGo = false;
        yield return new WaitForSeconds(this.timeToWaitOnWin);
        this.readyToGo = true;
    }
}
