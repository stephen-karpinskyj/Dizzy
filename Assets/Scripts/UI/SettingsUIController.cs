using UnityEngine;
using UnityEngine.UI;

public class SettingsUIController : MonoBehaviour
{
    [SerializeField]
    private Text soundText;

    [SerializeField]
    private Text spinDirectionText;

    [SerializeField]
    private Text graphicsQualityText;

    [SerializeField]
    private Button resetProgressButton;

    [SerializeField]
    private GameObject resetProgressConfirmParent;

    [SerializeField]
    private Color resetProgressEnabledButtonColor;

    [SerializeField]
    private Color resetProgressDisabledButtonColor;

    [SerializeField]
    private string soundEnabledText = "SOUND: ON";

    [SerializeField]
    private string soundDisabledText = "SOUND: OFF";

    [SerializeField]
    private string spinDirectionCCWText = "INITIAL SPIN: CCW";

    [SerializeField]
    private string spinDirectionCWText = "INITIAL SPIN: CW";

    [SerializeField]
    private string graphicsQualityHighText = "GRAPHICS: HI";

    [SerializeField]
    private string graphicsQualityLowText = "GRAPHICS: LO";

    [SerializeField]
    private GameObject graphicsQualityHighParent;

    private bool isResetProgressConfirmShowing = false;

    private void Awake()
    {
        Debug.Assert(this.soundText);
        Debug.Assert(this.spinDirectionText);
        Debug.Assert(this.graphicsQualityText);
        Debug.Assert(this.resetProgressButton);
        Debug.Assert(this.resetProgressConfirmParent);

        this.UpdateSoundToggle(StateManager.Instance.SoundEnabled);
        this.UpdateSpinDirectionToggle(StateManager.Instance.SpinDirectionCCW);
        this.UpdateGraphicsQualityToggle(StateManager.Instance.GraphicsQualityHigh);
        this.UpdateResetProgressConfirm(false);
    }

    public void OnSoundToggle()
    {
        var enabled = !StateManager.Instance.SoundEnabled;
        StateManager.Instance.SoundEnabled = enabled;

        this.UpdateSoundToggle(enabled);
    }

    public void OnSpinDirectionToggle()
    {
        var ccw = !StateManager.Instance.SpinDirectionCCW;
        StateManager.Instance.SpinDirectionCCW = ccw;

        this.UpdateSpinDirectionToggle(ccw);
    }
    
    public void OnGraphicsQualityToggle()
    {
        var hi = !StateManager.Instance.GraphicsQualityHigh;
        StateManager.Instance.GraphicsQualityHigh = hi;
        
        this.UpdateGraphicsQualityToggle(hi);
    }

    public void OnResetProgress()
    {
        this.UpdateResetProgressConfirm(!this.isResetProgressConfirmShowing);
    }

    public void OnResetProgressConfirm()
    {
        GameManager.Instance.ResetProgress();

        this.UpdateResetProgressConfirm(false);
    }

    private void UpdateSoundToggle(bool enabled)
    {
        this.soundText.text = enabled ? this.soundEnabledText : this.soundDisabledText;
    }

    private void UpdateSpinDirectionToggle(bool ccw)
    {
        this.spinDirectionText.text = ccw ? this.spinDirectionCCWText : this.spinDirectionCWText;
    }
    
    private void UpdateGraphicsQualityToggle(bool hi)
    {
        this.graphicsQualityText.text = hi ? this.graphicsQualityHighText : this.graphicsQualityLowText;
        
        this.graphicsQualityHighParent.SetActive(hi);
    }

    private void UpdateResetProgressConfirm(bool show)
    {
        this.isResetProgressConfirmShowing = show;

        var colours = this.resetProgressButton.colors;
        colours.normalColor = show ? this.resetProgressEnabledButtonColor : this.resetProgressDisabledButtonColor;
        colours.highlightedColor = show ? this.resetProgressEnabledButtonColor : this.resetProgressDisabledButtonColor;
        colours.pressedColor = show ? this.resetProgressEnabledButtonColor : this.resetProgressDisabledButtonColor;
        this.resetProgressButton.colors = colours;

        this.resetProgressConfirmParent.SetActive(this.isResetProgressConfirmShowing);
    }
}
