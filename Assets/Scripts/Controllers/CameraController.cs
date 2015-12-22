using System.Collections;
using UnityEngine;

public class CameraController : BehaviourSingleton<CameraController>
{
    private Vector3 originalCamPos;
    
    protected override void Awake()
    {
        base.Awake();

        instance = this;

        this.originalCamPos = this.transform.position;
    }

    public void Shake(float duration, float magnitude)
    {
        this.StartCoroutine(this.ShakeCoroutine(duration, magnitude));
    }

    /// <remarks>Source: http://unitytipsandtricks.blogspot.co.nz/2013/05/camera-shake.html</remarks>
    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        var elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;          

            var percentComplete = elapsed / duration;
            var damper = 1f - Mathf.Clamp(4f * percentComplete - 3f, 0f, 1f);

            var x = Mathf.PerlinNoise(Random.value * 100f, Random.value * 100f) * 2f - 1f;
            var y = Mathf.PerlinNoise(Random.value * 100f, Random.value * 100f) * 2f - 1f;
            x *= magnitude * damper;
            y *= magnitude * damper;

            this.transform.position = new Vector3(x, y, originalCamPos.z);

            yield return null;
        }

        this.transform.position = originalCamPos;
    }
}
