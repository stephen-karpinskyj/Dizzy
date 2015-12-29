using UnityEngine;

[RequireComponent(typeof(SpinGameObject))]
public class RandomizeSpin : MonoBehaviour
{
    [SerializeField]
    private Vector3 minimum = new Vector3(0f, 50f, 0f);

    [SerializeField]
    private Vector3 maximum = new Vector3(0f, 100f, 0f);
        
    private void Awake()
    {
        var spin = this.GetComponent<SpinGameObject>();

        var x = Random.Range(this.minimum.x, this.maximum.x) * (Random.value < 0.5f ? -1f : 1f);
        var y = Random.Range(this.minimum.y, this.maximum.y) * (Random.value < 0.5f ? -1f : 1f);
        var z = Random.Range(this.minimum.z, this.maximum.z) * (Random.value < 0.5f ? -1f : 1f);

        spin.SetMagnitude(new Vector3(x, y, z));
    }
}
