using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public Player player;
    public bool HP = false;
    public bool AP = false;

    public Vector3 HPStartDisplacement;
    public float HPDisplacement = 0;
    public GameObject HPTile;
    private GameObject[] HPTileSet;
    private float oldHP;

    public Vector3 APStartDisplacement;
    public float APDisplacement = 10;
    public GameObject APTile;
    private GameObject[] APTileSet;
    private int oldAP;

    // Start is called before the first frame update
    void Start()
    {
        if (!player)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }
        if (AP)
        {
            APTileSet = new GameObject[player.AP];
        }
        if (HP)
        {
            HPTileSet = new GameObject[(int)player.HP];
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (AP)
        {
            if (player.AP != oldAP)
            {
                oldAP = player.AP;
                Vector3 temp = transform.position + APStartDisplacement;
                foreach (GameObject G in APTileSet)
                {
                    Destroy(G);
                }
                APTileSet = new GameObject[player.AP];

                for (int i = 0; i < oldAP; i++)
                {
                    temp.x += APDisplacement;
                    APTileSet[i] = Instantiate(APTile, temp, Quaternion.identity, transform);
                }
            }
        }
        if (HP)
        {
            if (player.HP != oldHP)
            {
                oldHP = player.HP;
                Vector3 temp = transform.position + HPStartDisplacement;
                foreach (GameObject G in HPTileSet)
                {
                    Destroy(G);
                }
                HPTileSet = new GameObject[(int)player.HP];

                for (int y = 0; y < oldHP / 2; y++)
                {
                    temp.y += HPDisplacement;
                    for (int i = 0; i < 2; i++)
                    {
                        temp.x += HPDisplacement * i;
                        HPTileSet[(y * 2) + i] = Instantiate(HPTile, temp, Quaternion.identity, transform);
                        if ((y * 2) + i == oldHP)
                        {
                            break;
                        }
                    }
                    temp.x = transform.position.x + HPStartDisplacement.x;
                }
            }
        }
    }
}
