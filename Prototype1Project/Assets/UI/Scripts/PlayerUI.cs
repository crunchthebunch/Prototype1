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
    public float HPDisplacementDivision;
    public GameObject HPTile;
    private GameObject[] HPTileSet;
    private float oldHP;

    public Vector3 APStartDisplacement;
    public float APDisplacementDivision;
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
                Vector3 temp = transform.position;
                temp.x += APStartDisplacement.x * transform.root.localScale.x;
                foreach (GameObject G in APTileSet)
                {
                    Destroy(G);
                }
                APTileSet = new GameObject[player.AP];

                for (int i = 0; i < oldAP; i++)
                {
                    temp.x += APDisplacementDivision * transform.root.localScale.x;
                    APTileSet[i] = Instantiate(APTile, temp, Quaternion.identity, transform);
                }
            }
        }
        if (HP)
        {
            if (player.HP != oldHP && player.HP >= 0)
            {
                oldHP = player.HP;
                Vector3 temp = transform.position;
                temp.x += HPStartDisplacement.x * transform.root.localScale.x;
                temp.y += HPStartDisplacement.y * transform.root.localScale.y;
                foreach (GameObject G in HPTileSet)
                {
                    Destroy(G);
                }
                HPTileSet = new GameObject[(int)player.HP];

                for (int y = 0; y < oldHP / 2; y++)
                {
                    temp.y += HPDisplacementDivision * transform.root.localScale.y;
                    for (int i = 0; i < 2; i++)
                    {
                        temp.x += (HPDisplacementDivision * transform.root.localScale.y) * i;
                        HPTileSet[(y * 2) + i] = Instantiate(HPTile, temp, Quaternion.identity, transform);
                        if ((y * 2) + i == oldHP)
                        {
                            break;
                        }
                    }
                    temp.x = transform.position.x + (HPStartDisplacement.x * transform.root.localScale.x);
                }
            }
        }
    }
}
