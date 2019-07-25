using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InitiativeUI : MonoBehaviour
{
    public GameObject SelectTile;
    public GameObject InitiativeTile;
    public Vector3 StartLocation;
    public float Offset = 20;
    private GameObject[] InitiativeList;
    private int Selected;

    private GameManager Manager;
    private GameObject player;
    private int enemyNum;

    // Start is called before the first frame update
    void Start()
    {
        Manager = FindObjectOfType<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        InitiativeList = new GameObject[Manager.enemies.Length + 1];
        updateList();
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyNum != Manager.enemies.Length)
        {
            updateList();
        }
    }

    void updateList()
    {
        foreach (GameObject G in InitiativeList)
        {
            Destroy(G);
        }

        StartLocation.x = -((Manager.enemies.Length * Offset) / 2);

        InitiativeList = new GameObject[Manager.enemies.Length + 1];
        InitiativeList[0] = Instantiate(InitiativeTile, transform);
        InitiativeList[0].transform.localPosition = StartLocation;
        InitiativeList[0].GetComponent<CharacterInfo>().Referance = player;
        InitiativeList[0].GetComponentInChildren<Image>();
        InitiativeList[0].GetComponentInChildren<TextMeshProUGUI>().text = player.tag;

        int Initiative = 1;
        foreach (GameObject G in Manager.enemies)
        {
            InitiativeList[Initiative] = Instantiate(InitiativeTile, transform);
            InitiativeList[Initiative].transform.localPosition = new Vector3(StartLocation.x + (Offset * Initiative), StartLocation.y);
            InitiativeList[Initiative].GetComponent<CharacterInfo>().Referance = G;
            InitiativeList[Initiative].GetComponentInChildren<Image>();
            InitiativeList[Initiative].GetComponentInChildren<TextMeshProUGUI>().text = G.tag;
            Initiative++;
        }
        enemyNum = Manager.enemies.Length;
    }
}
