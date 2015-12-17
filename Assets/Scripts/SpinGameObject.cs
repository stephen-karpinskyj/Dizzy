using UnityEngine;

public class SpinGameObject : MonoBehaviour
{
    private enum Mode
    {
        Constant = 0,
        BySpeed,
    }

    [SerializeField]
    private Mode mode = Mode.Constant;

    [SerializeField]
    private Vector3 magnitude = Vector3.up;

    private Vector3 prevPos;

    private void Awake()
    {
        this.prevPos = this.transform.position;
    }

    private void Update()
    {
        switch (this.mode)
        {
            case Mode.Constant:
                {
                    this.transform.localRotation = this.transform.localRotation * Quaternion.Euler(Time.smoothDeltaTime * this.magnitude);
                }
                break;

            case Mode.BySpeed:
                {
                    var dist = Vector3.Distance(this.transform.position, this.prevPos);
                    this.transform.localRotation = this.transform.localRotation * Quaternion.Euler(Time.smoothDeltaTime * this.magnitude * dist);
                    this.prevPos = this.transform.position;
                }
                break;
        }
    }
}
