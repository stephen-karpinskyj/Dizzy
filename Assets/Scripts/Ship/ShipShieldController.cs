using UnityEngine;

public class ShipShieldController : MonoBehaviour
{
    [SerializeField]
    private Renderer rend;

    [SerializeField]
    private float showSpeed = 5f;

    [SerializeField]
    private float hideSpeed = 10f;

    [SerializeField]
    private AnimationCurve tweenCurve;

    [SerializeField]
    private string shaderParameterName = "_Strength";

    private float initialStrength;
    private Vector3 initialScale;

    private float currMultiplier;
    private float targetMultiplier;

    private void Awake()
    {
        this.initialStrength = this.rend.material.GetFloat(this.shaderParameterName);
        this.initialScale = this.transform.localScale;

        this.currMultiplier = 0f;
        this.targetMultiplier = 0f;

        this.UpdateVisuals();

        this.transform.localScale = Vector3.zero;
    }
    
    private void Update()
    {
        var diff = Mathf.Abs(this.targetMultiplier - this.currMultiplier);
        
        if (diff > 0.05f)
        {
            var dir = Mathf.Sign(this.targetMultiplier - this.currMultiplier);
            var speed = dir > 0 ? this.showSpeed : this.hideSpeed;
            this.currMultiplier += dir * speed * Time.smoothDeltaTime;
            this.UpdateVisuals();
        }
        else if (!Mathf.Approximately(this.targetMultiplier, this.currMultiplier))
        {
            this.currMultiplier = this.targetMultiplier;
            this.UpdateVisuals();
        }
    }

    public void Show(bool show)
    {
        this.targetMultiplier = show ? 1f : 0f;
    }

    private void UpdateVisuals()
    {
        var multiplier = this.tweenCurve.Evaluate(this.currMultiplier);
        this.transform.localScale = this.initialScale * multiplier;
        this.rend.material.SetFloat(this.shaderParameterName, this.initialStrength * multiplier);
    }
}
