using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
public class ChangeScene : MonoBehaviour
{

    AudioSource audioSource;

    public AudioClip backgroundMusic;

    public AudioClip playButtonSound;
    public AudioClip otherButtonSound;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = backgroundMusic;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void playGame()
    {
        audioSource.clip = playButtonSound;
        audioSource.Play();
        Invoke("transitionToLevel", 0.75f);
        
    }

    public void quitApplication()
    {
        audioSource.clip = otherButtonSound;
        audioSource.Play();
        Invoke("quitGame", 0.75f);
    }

    public void mainMenu()
    {
        audioSource.clip = otherButtonSound;
        audioSource.Play();
        Invoke("changeToMainMenu", 0.75f);
    }

    void changeToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
    void quitGame()
    {
        Application.Quit();
    }

    void transitionToLevel()
    {
        SceneManager.LoadScene("Callum level");
    }
}
