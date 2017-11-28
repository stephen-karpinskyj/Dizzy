using UnityEngine;

/// <remarks>Based on: http://wiki.unity3d.com/index.php?title=CameraFacingBillboard</remarks>
public class CameraFacingBillboard : MonoBehaviour
{
    [SerializeField]
    private Camera cam;

    [SerializeField]
    private bool x = true;

	[SerializeField]
	private bool y = true;

	[SerializeField]
	private bool z = true;

	private void Awake()
    {
        if (this.cam == null)
        {
            this.cam = Camera.main;
        }

        Debug.Assert(this.cam != null, this);
    }

    private void LateUpdate()
    {
        if (this.cam != null)
        {
            var oldEuler = this.transform.eulerAngles;
            
            this.transform.LookAt(this.transform.position + this.cam.transform.rotation * Vector3.forward,
                this.cam.transform.rotation * Vector3.up);

            var newEuler = this.transform.eulerAngles;

			if (!x) newEuler.x = oldEuler.x;
			if (!y) newEuler.y = oldEuler.y;
			if (!z) newEuler.z = oldEuler.z;

            this.transform.eulerAngles = newEuler;
        }
    }
}