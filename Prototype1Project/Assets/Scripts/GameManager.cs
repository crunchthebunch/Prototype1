using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public Player player;
    public GameObject enemy;
    public TopDownCamera mainCamera;
    public Camera playerCam;
    GameObject[] enemies;
    public int initiativeCount = 0;
    public bool camSwitch;
    public bool isShooting;

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

        camSwitch = false;
        player = FindObjectOfType<Player>();
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        for (int i = 0; i < enemies.Length; i++)
        {
            HumanAI ai = enemies[i].GetComponent<HumanAI>();
            ai.initiative = i + 1;
        }
    }

    void Update()
    { 
        if (Input.GetMouseButtonDown(1) && initiativeCount == 0 && !player.startedMoving)
        {
            EndTurn();
        }

        if (Input.GetKeyDown(KeyCode.Space) && !player.startedMoving)
        {
            if (!camSwitch)
            {
                playerState = PlayerState.SHOOTING;
            }
            else
            {
                playerState = PlayerState.MOVING;
            }
            camSwitch = !camSwitch;
            playerCam.gameObject.SetActive(camSwitch);
            mainCamera.gameObject.SetActive(!camSwitch);
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
