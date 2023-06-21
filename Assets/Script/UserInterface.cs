using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UserInterface : MonoBehaviour
{
    public void ExitGame()
    {
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
    public void StartGame()
    {
        SceneManager.LoadScene("angry");
    }
    public void PauseGame()
    {
        SceneManager.LoadScene(SceneManager.GetSceneByName("angry").buildIndex - 1);
        //Time.timeScale = 0;
    }
    public void ResumeGame()
    {
        Time.timeScale = 1;
    }
}
