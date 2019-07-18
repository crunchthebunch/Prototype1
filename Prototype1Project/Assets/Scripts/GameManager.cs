using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public Player player;
    public GameObject enemy;
    public TopDownCamera mainCamera;
    GameObject[] enemies;
    public static int initiativeCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        for (int i = 0; i < enemies.Length; i++)
        {
            HumanAI ai = enemies[i].GetComponent<HumanAI>();
            ai.initiative = i + 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (initiativeCount >= enemies.Length + 1)
        //{
        //    initiativeCount = 0;
        //    mainCamera.changeTarget(player.gameObject);
        //    player.AP = 10;
        //}

        if (Input.GetMouseButtonDown(1) && initiativeCount == 0)
        {
            EndTurn();
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
