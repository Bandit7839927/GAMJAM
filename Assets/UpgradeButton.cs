using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{

    [Header("Components de la UI")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descText;
    public Image iconImage;
    public Image buttonBackground;
    // Variables internes per saber què fa aquest botó concret
    private UpgradeData myUpgrade;
    private levelUpHandler manager;

    // Aquesta funció la crida el Manager per "pintar" la carta
    public void Setup(UpgradeData data, levelUpHandler mgr)
    {
        myUpgrade = data;
        manager = mgr;

        // Omplim els textos i imatges
        if(titleText != null) titleText.text = data.upgradeName;
        if(descText != null) descText.text = data.description;
        if(iconImage != null && data.icon != null) iconImage.sprite = data.icon;
        if(buttonBackground != null && data.backgroundColor != null) buttonBackground.color = data.backgroundColor;
    }

    // Aquesta funció l'has de connectar al "On Click()" del botó a Unity
    public void OnClick()
    {
        if(manager != null)
        {
            manager.ApplyUpgrade(myUpgrade);
        }
    }
}