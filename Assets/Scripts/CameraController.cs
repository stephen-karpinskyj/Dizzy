using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Camera cam;

    [SerializeField]
    private AnimationCurve sizeOffsetCurve;

    [SerializeField]
    private Vector2 sizeOffsetRange = new Vector2(0.5f, 1f);

    [SerializeField, Range(0f, 2f)]
    private float maxSizeOffset = 0.3f;

    [SerializeField]
    private float sizeOffsetToMultiplier = 2f;

    [SerializeField]
    private float sizeOffsetFromMultiplier = 4f;

    private Vector3 originalCamPos;
    private float originalSize;

    private Vector3 shakeOffset;
    private float targetHeight;

    private static CameraController instance;

    private void Awake()
    {
        Object.DontDestroyOnLoad(this.transform.root.gameObject);
        instance = this;

        this.originalCamPos = this.transform.position;
        this.shakeOffset = Vector3.zero;

        this.originalSize = this.cam.orthographicSize;
        this.targetHeight = 0;
    }

    private void Update()
    {
        this.transform.position = this.originalCamPos + this.shakeOffset;

        var speedPercentage = LevelManager.Instance.Ship.SpeedPercentage;
        var curveValue = (speedPercentage - this.sizeOffsetRange.x) / (this.sizeOffsetRange.y - this.sizeOffsetRange.x);
        this.targetHeight = this.originalSize + (this.sizeOffsetCurve.Evaluate(curveValue) * this.maxSizeOffset);
        var offsetMultiplier = this.targetHeight < this.cam.orthographicSize ? this.sizeOffsetFromMultiplier : this.sizeOffsetToMultiplier;
        this.cam.orthographicSize = Mathf.Lerp(this.cam.orthographicSize, this.targetHeight, Time.smoothDeltaTime * offsetMultiplier);
    }

    public static void Shake(float duration, float magnitude)
    {
        if (!instance)
        {
            return;
        }
        
        instance.StartCoroutine(instance.ShakeCoroutine(duration, magnitude));
    }

    /// <remarks>Based on: http://unitytipsandtricks.blogspot.co.nz/2013/05/camera-shake.html</remarks>
    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        var elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;          

            var percentComplete = elapsed / duration;
            var damper = 1f - Mathf.Clamp(4f * percentComplete - 3f, 0f, 1f);

            var circle = Random.insideUnitCircle * magnitude * damper;
            this.shakeOffset = new Vector3(circle.x, circle.y, this.originalCamPos.z);

            yield return null;
        }

        this.shakeOffset = Vector3.zero;
    }
}
