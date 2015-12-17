using System.Collections;
using UnityEngine;

public class CameraController : BehaviourSingleton<CameraController>
{
    protected override void Awake()
    {
        base.Awake();

        instance = this;
    }

    public void Shake(float duration, float magnitude)
    {
        this.StartCoroutine(this.ShakeCoroutine(duration, magnitude));
    }

    /// <remarks>Source: http://unitytipsandtricks.blogspot.co.nz/2013/05/camera-shake.html</remarks>
    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        var elapsed = 0f;

        var originalCamPos = this.transform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;          

            var percentComplete = elapsed / duration;
            var damper = 1f - Mathf.Clamp(4f * percentComplete - 3f, 0f, 1f);

            // map value to [-1, 1]
            var x = Random.value * 2f - 1f;
            var y = Random.value * 2f - 1f;
            x *= magnitude * damper;
            y *= magnitude * damper;

            this.transform.position = new Vector3(x, y, originalCamPos.z);

            yield return null;
        }

        this.transform.position = originalCamPos;
    }
}
