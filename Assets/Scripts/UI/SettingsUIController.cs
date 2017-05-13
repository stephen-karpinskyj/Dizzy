using UnityEngine;
using UnityEngine.UI;

public class SettingsUIController : MonoBehaviour
{
    [SerializeField]
    private Text soundText;

    [SerializeField]
    private Text spinDirectionText;

    [SerializeField]
    private Text graphicsModeText;
    
    [SerializeField]
    private Text fpsText;

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
    private string fpsEnabledText = "FPS: ON";

    [SerializeField]
    private string fpsDisabledText = "FPS: OFF";
    
    [SerializeField]
    private Text fpsParent;

    private bool isResetProgressConfirmShowing = false;

    private void Awake()
    {
        Debug.Assert(this.soundText);
        Debug.Assert(this.spinDirectionText);
        Debug.Assert(this.graphicsModeText);
        Debug.Assert(this.resetProgressButton);
        Debug.Assert(this.resetProgressConfirmParent);

        this.UpdateSoundToggle(StateManager.Instance.SoundEnabled);
        this.UpdateSpinDirectionToggle(StateManager.Instance.SpinDirectionCCW);
        this.UpdateFpsToggle(StateManager.Instance.FpsEnabled);
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
    
    public void OnGraphicsModeToggle()
    {
    }
    
    public void OnFpsToggle()
    {
        var fps = !StateManager.Instance.FpsEnabled;
        StateManager.Instance.FpsEnabled = fps;
        
        this.UpdateFpsToggle(fps);
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

    private void UpdateFpsToggle(bool fps)
    {
        this.fpsText.text = fps ? this.fpsEnabledText : this.fpsDisabledText;
        this.fpsParent.enabled = fps;
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
