using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class MainMenu_options : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void PlayButton(){
        SceneManager.LoadSceneAsync(1);
    }

    public void ResetProgressButton()
    {
        // Això esborra ABSOLUTAMENT TOT (millores, màscares, nivells)
        PlayerPrefs.DeleteAll();
        Debug.Log("Progrés esborrat per a una nova experiència des de zero.");
        
        // Opcional: Recarregar l'escena per visualitzar els canvis si cal
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitButton(){
        Application.Quit();
    }
}