using UnityEngine;
using UnityEngine.UI;

public class MedalUIController : MonoBehaviour
{
    [SerializeField]
    private Text timeText;
    
    [SerializeField]
    private Image image;

    [SerializeField]
    private Sprite missingSprite;

    [SerializeField]
    private Color missingColor = new Color(58/255f, 58/255f, 58/255f, 1f);

    [SerializeField]
    private Sprite earntSprite;

    [SerializeField]
    private Color earntColor = Color.white;

    private void Awake()
    {
        Debug.Assert(this.timeText);
        Debug.Assert(this.image);
        Debug.Assert(this.missingSprite);
        Debug.Assert(this.earntSprite);
    }

    public void UpdateTime(float time)
    {
        this.timeText.text = FormattingUtility.VariableDPTimeToString(time);
    }

    public void UpdateEarnt(bool earnt)
    {
        this.image.sprite = earnt ? this.earntSprite : this.missingSprite;
        this.image.color = earnt ? this.earntColor : this.missingColor;
    }
}
