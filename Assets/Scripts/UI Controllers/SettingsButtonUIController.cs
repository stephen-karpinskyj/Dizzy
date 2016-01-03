using UnityEngine;
using UnityEngine.UI;

public class SettingsButtonUIController : MonoBehaviour
{
    [SerializeField]
    private GameObject parent;

    [SerializeField]
    private Button button;

    [SerializeField]
    private Color enabledColor;

    [SerializeField]
    private Color disabledColor;

    private bool isSettingsPanelShowing = false;

    private void Awake()
    {
        Debug.Assert(this.parent);
        Debug.Assert(this.button);

        this.UpdateShow(false);
    }

    public void Toggle()
    {
        UpdateShow(!this.isSettingsPanelShowing);
    }

    private void UpdateShow(bool show)
    {
        this.isSettingsPanelShowing = show;

        var colours = this.button.colors;
        colours.normalColor = show ? this.enabledColor : this.disabledColor;
        colours.highlightedColor = show ? this.enabledColor : this.disabledColor;
        colours.pressedColor = show ? this.enabledColor : this.disabledColor;
        this.button.colors = colours;

        this.parent.SetActive(show);
    }
}
