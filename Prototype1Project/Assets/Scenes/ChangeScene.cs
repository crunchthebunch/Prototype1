using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
public class ChangeScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void playGame()
    {
        SceneManager.LoadScene("Callum level");
    }

    public void quitApplication()
    {
        Application.Quit();
    }

    public void mainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
