using UnityEngine;
using UnityEngine.UI;

public class ExplorationBeaconUIController : MonoBehaviour
{
    [SerializeField]
    private string purchasePrefix = "ACTIVATE BEACON: ";
    
    [SerializeField]
    private Text purchaseText;
    
    [SerializeField]
    private Button purchaseButton;
    
    [SerializeField]
    private Color unpurchaseableColor;
    
    [SerializeField]
    private Color purchaseableColor;
        
    public void UpdateAll(ExplorationBeaconData data, bool isPurchased, bool isPurchaseable)
    {
        if (isPurchased)
        {
            this.purchaseButton.gameObject.SetActive(false);
        }
        else
        {
            this.purchaseButton.gameObject.SetActive(true);
            
            var junkCount = FormattingUtility.JunkCountToString(data.ActivationCost);
            this.purchaseText.text = string.Format("{0}{1}", this.purchasePrefix, junkCount);
            
            this.purchaseText.color = isPurchaseable ? this.purchaseableColor : this.unpurchaseableColor;
            this.purchaseButton.interactable = isPurchaseable;
        }
    }
    
    public void OnPurchaseButtonPress()
    {
        GameManager.Instance.HandleBeaconPurchase();
    }
}
