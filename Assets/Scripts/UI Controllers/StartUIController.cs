using UnityEngine;
using UnityEngine.UI;

public class StartUIController : MonoBehaviour
{
    [SerializeField]
    private string message = "<color={0}>Tap to thrust</color>";

    [SerializeField]
    private Text text;

    [SerializeField]
    private string onColor = "#cdcdcdff";

    [SerializeField]
    private string offColor = "#ffa500ff";

    private void Update()
    {
        this.text.text = string.Format(this.message, Time.time % 1f > 0.5f ? this.offColor : this.onColor);
    }
}
