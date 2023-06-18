using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour
{
    public InputField inputField;
    public GameObject box;
    Vector3 position = new Vector3(100f, 2f, 10f);
    public void OnValueChanged()
    {
        string inputString = inputField.text;
        int inputValue;
        bool isInputValid = int.TryParse(inputString, out inputValue);
        if (isInputValid)
        {
            for (int i = 0; i < inputValue; i++)
            {
             
                if (i % 10 == 0)
                {
                    position.z -= 10f;
                    position.x -= 250f;
                }
                Instantiate(box, position, Quaternion.identity);
                position.x += 25f;
            }
        }
        else
        {
            Debug.Log("**Input Not Valide**");
        }
    }
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
    }
}
