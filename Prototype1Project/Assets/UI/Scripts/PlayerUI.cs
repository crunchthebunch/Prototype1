using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public TextMeshProUGUI AP;
    public Player player;
    public Transform Camera;
    public Transform Piviot;

    public Vector3 HPTileStart;
    public GameObject HPTile;
    private GameObject[] HPTileSet;
    private float oldHP;

    public Vector3 APTileStart;
    public GameObject APTile;
    private GameObject[] APTileSet;
    private int oldAP;

    // Start is called before the first frame update
    void Start()
    {
        APTileSet = new GameObject[player.AP];
        HPTileSet = new GameObject[(int)player.HP];
    }

    // Update is called once per frame
    void Update()
    {
        if (Camera && Piviot)
        {
            Piviot.rotation = Camera.rotation;
        }
        if (player.AP != oldAP)
        {
            oldAP = player.AP;

            Vector3 temp = APTileStart;
            float tempWidth = 80;

            foreach (GameObject G in APTileSet)
            {
                Destroy(G);
            }
            APTileSet = new GameObject[player.AP];

            for (int i = 0; i < oldAP; i++)
            {
                APTileSet[i] = Instantiate(APTile, transform);
                APTileSet[i].transform.localPosition = temp;
                temp.x += tempWidth;
            }
        }
        if (player.HP != oldHP)
        {
            oldHP = player.HP;
            AP.text = "HP: " + player.HP;
            float tempWidth = 80;
            Vector3 temp = HPTileStart;
            foreach (GameObject G in HPTileSet)
            {
                Destroy(G);
            }
            HPTileSet = new GameObject[(int)player.HP];

            for (int y = 0; (y * 10) < oldHP; y++)
            {
                temp.x = HPTileStart.x;
                temp.y = (30 * y) + HPTileStart.y;
                for (int x = 0; x < 10; x++)
                {
                    if ((y * 10) + x == oldHP)
                    {
                        break;
                    }
                    HPTileSet[(y * 10) + x] = Instantiate(HPTile, transform);
                    HPTileSet[(y * 10) + x].transform.localPosition = temp;
                    temp.x += tempWidth;
                }
            }
        }
    }
}
