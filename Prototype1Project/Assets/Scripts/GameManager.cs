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
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(1))
        {
            if (initiativeCount == enemies.Length)
            {
                initiativeCount = 0;
                mainCamera.changeTarget(player.gameObject);
                player.AP = 10;
                
            }
            else
            {
                mainCamera.changeTarget(enemies[initiativeCount]);
                initiativeCount++;
            }
        }
    }
}
