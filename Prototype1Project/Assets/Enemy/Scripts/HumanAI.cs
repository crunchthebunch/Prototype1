using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HumanAI : MonoBehaviour
{
    public int hp;
    public int ap;
    public int speed;

    int currentPointID; //id of the coverPoint taken on the current cover, -1 if not in cover

    bool isMyTurn;
    bool isAggro;
    bool isWandering;

    private CoverObject currentCover;
    private CoverObject[] coverObjects;
    public GameObject player;

    Vector3 moveTo;

    private enum AI
    {
        idle,
        wander,
        search,
        engage
    };

    AI state;
    NavMeshAgent agent;

    void Start()
    {
        isAggro = true;
        state = AI.engage;
        isMyTurn = false;
        agent = GetComponent<NavMeshAgent>();
        if (coverObjects == null)
        {
            coverObjects = FindObjectsOfType<CoverObject>();
        }
    }

    bool CheckTurn()
    {
        return true;
    }

    void Update()
    {
        if (!isMyTurn && CheckTurn() == true)
        {
            isMyTurn = true;
            if (isAggro)
            {
                CombatBehaviour();
            }
        }

        if (isMyTurn)
        {
            if (agent.remainingDistance <= 0.1f)
            {
                isMyTurn = false;
            }
        }
    }

    void CombatBehaviour()
    {
        switch (state)
        {
            case AI.idle:  //Idling
                {
                    break;
                }
            case AI.wander: //Wandering
                {
                    Wander();
                    break;
                }
            case AI.search:
                {
                    Search(); //Searching for enemies
                    break;
                }
            case AI.engage:
                {
                    Engage(); //Engage with enemies
                    break;
                }
            default:
                break;
        }
    }

    void Idle() //Idling
    {
    }

    void Wander() //Wandering
    {

        isMyTurn = false;
    }

    void Search() //Searching for enemies
    {

    }

    void Engage() //Engaging with enemies
    {
        Vector3 cover = FindCover();
        if (cover != null)
        {
            Move(cover);
        }
    }

    Vector3 FindCover() //Finding the nearest reachable cover object
    {
        Vector3 bestPoint = transform.position;
        CoverObject bestCover = null;
        float bestCoverDist = 100.0f;
        float bestPointDist = 0.0f;

        //------------ First find the nearest cover object ------------
        for (int i = 0; i < coverObjects.Length; i++)
        {
            CoverObject cover = coverObjects[i];

            NavMeshPath path = new NavMeshPath();
            agent.CalculatePath(cover.transform.position, path);

            //If the cover is within reachable distance
            if (agent.pathStatus == NavMeshPathStatus.PathComplete)
            {
                if (agent.remainingDistance <= ap) //If the cover object is reachable with current AP
                {
                    float dist = Vector3.Distance(transform.position, cover.transform.position);

                    if (dist < bestCoverDist) //If the distance is the best distance
                    {
                        bestCoverDist = dist;
                        bestCover = cover;
                        currentCover = bestCover;
                    }
                }
            }
        }

        //------------- Then find the furthest cover point on the cover object from the enemy ---------
        if (bestCover != null)
        {
            //Setting the current cover point to be free
            currentCover.pointTakenList[currentPointID] = false;
            currentPointID = -1;

            int bestPointID = -1;
            for (int id = 0; id < bestCover.coverPoints.Length; id++)
            {
                Vector3 coverPoint = bestCover.coverPoints[id];

                float dist = Vector3.Distance(coverPoint, player.transform.position);

                if (dist > bestPointDist && bestCover.pointTakenList[id] == false)
                {
                    bestPointID = id;
                    bestPointDist = dist;
                    bestPoint = coverPoint;
                }
            }
            currentPointID = bestPointID;       
            currentCover.pointTakenList[bestPointID] = true;
        }

        return bestPoint;
    }

    void Move(Vector3 destination) 
    {
        if (agent.remainingDistance <= ap)
        {
            agent.destination = destination;
        }
    }


}
