using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MaskButton : MonoBehaviour
{

    [Header("Components de la UI")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descText;
    public Image iconImage;

    public Image mascara_UI;
    // Variables internes per saber què fa aquest botó concret
    private MaskData myMask;
    private MaskHandler manager;

    // Aquesta funció la crida el Manager per "pintar" la carta
    public void Setup(MaskData data, MaskHandler mgr)
    {
        myMask = data;
        manager = mgr;

        // Omplim els textos i imatges
        if(titleText != null) titleText.text = data.maskName;
        if(descText != null) descText.text = data.description;
        if(iconImage != null && data.menuIcon != null) iconImage.sprite = data.menuIcon;
    }

    // Aquesta funció l'has de connectar al "On Click()" del botó a Unity
    public void OnClick()
    {
        if(manager != null)
        {
            manager.ApplyUpgrade_Mask(myMask);
        }
    }
}