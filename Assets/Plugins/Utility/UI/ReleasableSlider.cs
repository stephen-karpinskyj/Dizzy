using UnityEngine;
using UnityEngine.UI;

public class ReleasableSlider : Slider
{
    [SerializeField]
    private SliderEvent onReleased;

    public override void OnPointerUp(UnityEngine.EventSystems.PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        this.onReleased.Invoke(this.value);
    }
}
