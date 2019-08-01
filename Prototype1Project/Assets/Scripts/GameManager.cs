using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public Player player;
    public GameObject enemy;
    public TopDownCamera mainCamera;
    public Camera playerCam;
    [HideInInspector] public GameObject[] enemies;
    public GameObject[] aggroEnemies;
    public int initiativeCount = 0;
    public bool camSwitch;
    public bool isShooting;

    AudioSource audioSource;
    public AudioClip backGroundMusicLevel;
    public AudioClip backGroundAggroMusic;
    public AudioClip playerDeathSound;

    public bool isPlayerDead;
    public bool isPlayingAudio;
    public bool isAnyAggro;

    public enum PlayerState
    {
        MOVING,
        SHOOTING
    };

    public PlayerState playerState;

    void Start()
    {
        //isShooting = false;
        playerState = PlayerState.MOVING;
        isPlayerDead = false;
        isPlayingAudio = false;
        isAnyAggro = false;
        camSwitch = false;
        player = FindObjectOfType<Player>();
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = backGroundMusicLevel;
        audioSource.volume = 0.3f;
        audioSource.Play();

        for (int i = 0; i < enemies.Length; i++)
        {
            HumanAI ai = enemies[i].GetComponent<HumanAI>();
            ai.initiative = i + 1;
        }
    }

    void Update()
    { 
        if (Input.GetKeyDown(KeyCode.Space) && initiativeCount == 0 && !player.startedMoving && playerState != PlayerState.SHOOTING)
        {
            EndTurn();
        }

        if (Input.GetMouseButton(1) && !player.startedMoving)
        {
            playerState = PlayerState.SHOOTING;
            playerCam.gameObject.SetActive(true);
            mainCamera.gameObject.SetActive(false);
        }
        else
        {
            playerState = PlayerState.MOVING;
            playerCam.gameObject.SetActive(false);
            mainCamera.gameObject.SetActive(true);
        }

        if (isPlayerDead)
        {
            audioSource.clip = playerDeathSound;
            audioSource.Play();
            isPlayerDead = false;
        }

        isAnyAggro = false;

        for (int i = 0; i < enemies.Length; i++)
        {
            HumanAI ai = enemies[i].GetComponent<HumanAI>();
            if (ai.isAggro)
            {
                isAnyAggro = true;   
            }
            else
            {

            }

        }

        if (isAnyAggro)
        {
            if (isPlayingAudio)
            {
                audioSource.clip = backGroundAggroMusic;
                audioSource.volume = 1.0f;
                audioSource.Play();
                isPlayingAudio = false;
            }
           
        }
        else
        {
            if (!isPlayingAudio)
            {
                audioSource.clip = backGroundMusicLevel;
                audioSource.volume = 0.3f;
                audioSource.Play();
                isPlayingAudio = true;
            }
            
        }
        

    }

    public void EndTurn()
    {
        if (initiativeCount < enemies.Length)
        {
            mainCamera.changeTarget(enemies[initiativeCount]);
            initiativeCount++;
        }
        else
        {
            initiativeCount = 0;
            mainCamera.changeTarget(player.gameObject);
            player.AP = 10;
        }
    }
}
