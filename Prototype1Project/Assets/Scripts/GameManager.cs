using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public GameObject player;
    public GameObject enemy;
    public TopDownCamera mainCamera;
    GameObject[] enemies;
    public int initiativeCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
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
                mainCamera.changeTarget(player);
            }
            else
            {
                mainCamera.changeTarget(enemies[initiativeCount]);
                initiativeCount++;
            }
        }
    }
}
