using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MaskButton : MonoBehaviour
{
//ola bsoki
    [Header("Components de la UI")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descText;
    public Image iconImage;

    public Image mascara_UI;
    // Variables internes per saber què fa aquest botó concret
    private MaskData myMask;
    private MaskHandler manager;

    public Sprite iconaInterrogant; // Assigna una imatge de "?" a l'inspector
    private bool estaBloquejat = false;

    public void Setup(MaskData data, MaskHandler mgr, bool desbloquejada)
    {
        myMask = data;
        manager = mgr;
        estaBloquejat = !desbloquejada;

        if (desbloquejada) {
            if(titleText != null) titleText.text = data.maskName;
            if(descText != null) descText.text = data.description;
            if(iconImage != null) iconImage.sprite = data.menuIcon;
            GetComponent<Button>().interactable = true; // El botó es pot clicar
        } else {
            if(titleText != null) titleText.text = "???";
            if(descText != null) descText.text = "Encara no has trobat aquesta màscara.";
            if(iconImage != null) iconImage.sprite = iconaInterrogant;
            GetComponent<Button>().interactable = false; // No es pot triar
        }
    }

    public void OnClick() {
        if(!estaBloquejat && manager != null) {
            manager.ApplyUpgrade_Mask(myMask);
        }
    }
}