using UnityEngine;
using UnityEngine.UI;

public class HeaderUIController : MonoBehaviour
{
    [SerializeField]
    private Text levelTitleText;
    
    private void Awake()
    {
        Debug.Assert(this.levelTitleText);
    }
    
    public void UpdateLevelTitleText(string title)
    {
        this.levelTitleText.text = title;
    }
    
    public void OnPrevLevel()
    {
        GameManager.Instance.LoadNextLevel(false);
    }
    
    public void OnNextLevel()
    {
        GameManager.Instance.LoadNextLevel(true);
    }
}
