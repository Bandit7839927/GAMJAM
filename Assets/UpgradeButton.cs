using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    [Header("Components de la UI")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descText;
    public Image iconImage;

    // CORRECCIÓ: Referència al component Image del fons, no a un color simple
    public Image buttonBackgroundImage; 

    private UpgradeData myUpgrade;
    private levelUpHandler manager;

    public void Setup(UpgradeData data, levelUpHandler mgr)
    {
        myUpgrade = data;
        manager = mgr;

        if(titleText != null) titleText.text = data.upgradeName;
        if(descText != null) descText.text = data.description;
        if(iconImage != null && data.icon != null) iconImage.sprite = data.icon;

        // APLICAR EL COLOR DEL SCRIPTABLE OBJECT
        if(buttonBackgroundImage != null)
        {
            buttonBackgroundImage.color = data.backgroundColor;
        }
    }

    public void OnClick()
    {
        if(manager != null)
        {
            manager.ApplyUpgrade(myUpgrade);
        }
    }
}