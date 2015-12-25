using System.Collections;
using UnityEngine;

public class CameraController : BehaviourSingleton<CameraController>
{
    private Vector3 originalCamPos;
    private Vector3 boostPos;
    private Vector3 shakePos;
    
    protected override void Awake()
    {
        base.Awake();

        instance = this;

        this.originalCamPos = this.transform.position;

        this.boostPos = Vector3.zero;
        this.shakePos = Vector3.zero;
    }

    private void Update()
    {
        this.transform.position = this.originalCamPos + this.shakePos + this.boostPos;
    }

    public void Shake(float duration, float magnitude)
    {
        this.StartCoroutine(this.ShakeCoroutine(duration, magnitude));
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
            this.shakePos = new Vector3(circle.x, circle.y, this.originalCamPos.z);

            yield return null;
        }

        this.shakePos = Vector3.zero;
    }
}
