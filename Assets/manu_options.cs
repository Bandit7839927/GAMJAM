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

    public void ExitButton(){
        Application.Quit();
    }
}