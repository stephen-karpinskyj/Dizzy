using UnityEngine;

public class OrbitGameObject : MonoBehaviour
{
    [SerializeField]
    private float speed = 20f;

    [SerializeField]
    private float angularSpeed = -15f;
    
    [SerializeField]
    private float gravitationalMagnitude;

    [SerializeField]
    private Transform gravitationalTarget;

    private Vector2 currDir;
    private Vector2 currVelocity;

    private void Update()
    {
        // Linear velocity
        {
            var targetDir = (Vector2)(this.gravitationalTarget.position - this.transform.position).normalized;
            this.currVelocity += targetDir * this.gravitationalMagnitude;
            
            var forwardVelocity = (Vector2)this.transform.right * this.speed;
            
            this.transform.position += (Vector3)(this.currVelocity + forwardVelocity) * Time.deltaTime;
        }
        
        // Angular velocity
        {
            var targetDir = (Vector2)(this.transform.position - this.gravitationalTarget.position).normalized;
            var currDir = this.transform.right;

            var angle = currDir.SignedAngle(targetDir);
            if (angle < 0f)
            {
                angle += 360f;
            }
            else if (angle > 360f)
            {
                angle -= 360f;
            }

            var euler = this.transform.localEulerAngles;
            euler.z += Mathf.Sign(angle) * Time.deltaTime * this.angularSpeed;
            this.transform.localEulerAngles = euler;
        }
    }
}
