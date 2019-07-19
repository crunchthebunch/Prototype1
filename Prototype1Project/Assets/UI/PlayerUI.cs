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

    public Vector3 APTileStart;
    public GameObject APTile;
    private GameObject[] APTileSet;
    private int oldAP;

    // Start is called before the first frame update
    void Start()
    {
        APTileSet = new GameObject[10];
    }

    // Update is called once per frame
    void Update()
    {
        Piviot.rotation = Camera.rotation;
        if (player.AP != oldAP)
        {
            oldAP = player.AP;
            AP.text = "AP: " + player.AP;

            Vector3 temp = APTileStart;
            float tempWidth = APTile.GetComponent<RectTransform>().rect.width;

            foreach (GameObject G in APTileSet)
            {
                Destroy(G);
            }

            for (int i = 0; i < oldAP; i++)
            {
                temp.x += tempWidth + 10;
                APTileSet[i] = Instantiate(APTile, transform);
                APTileSet[i].transform.localPosition = temp;
            }
        }
    }
}
