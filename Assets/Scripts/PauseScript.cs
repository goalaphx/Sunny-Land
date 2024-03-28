using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
   
    public void Pause(){
        pauseMenu.SetActive(true);
        Time.timeScale = 0;

    }
    public void MainMenu(){
        SceneManager.LoadScene("Main Menu");
        Time.timeScale = 1;
        
    }
    public void Continue(){
        pauseMenu.SetActive(false);
        Time.timeScale = 1;

    }
    public void RestartLevel(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
        
    }
    
}
