using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HumanAI : MonoBehaviour
{
    public int hp;
    public int ap;
    public int speed;
    public int initiative;

    int coverPointID; //id of the coverPoint taken on the current cover, -1 if not in cover

    bool isMyTurn;
    bool isAggro;
    bool isWandering;
    bool isEndingTurn;
    bool isShooting;

    float endTimer; //Timer for ending turn (used mostly for actions)

    private Vector3 moveDestination;
    private CoverObject currentCover;
    private CoverObject[] coverObjects;
    public GameObject gunObject;
    public GameObject gunSlot;
    GameObject player;

    public List<CoverPoint> viablePoints = new List<CoverPoint>(); //List of viable cover points this turn

    Guns gun;
    GameManager gameManager;

    Vector3 moveTo;
    Vector3 checkOrigin;
    Vector3 playerDirection;

    RaycastHit hit;

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
        player = GameObject.FindGameObjectWithTag("Player");
        gameManager = FindObjectOfType<GameManager>();
        moveDestination = transform.position;
        gun = gunObject.GetComponent<Guns>();
        endTimer = -1.0f;
        checkOrigin = transform.position;
        isShooting = false;
        if (coverObjects == null)
        {
            coverObjects = FindObjectsOfType<CoverObject>();
        }
    }

    bool CheckTurn()
    {
        if (gameManager.initiativeCount == initiative)
        {
            return true;
        }

        return false;
    }

    void EndTurn()
    {
        if (isMyTurn)
        {
            gameManager.EndTurn();
            isMyTurn = false;
        }
    }

    void Update()
    {
        //Checking for if it is this AI's turn
        if (!isMyTurn && CheckTurn() == true)
        {
            isMyTurn = true;
            isEndingTurn = false;
            endTimer = -1.0f;
            isShooting = false;
            gun.CanFire = false;
            if (isAggro)
            {
                CombatBehaviour();
            }
        }

        //This AI's turn
        if (isMyTurn)
        {
            gun.CanFire = true;
            //Ending turn if not moving and isEndingTurn is true
            if (!agent.pathPending)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
                        //End turn timer
                        if (endTimer != -1.0f)
                        {
                            if (endTimer > 0.0f)
                            {
                                endTimer -= Time.deltaTime;

                                CheckLineOfSight(transform.position);
                                if (isShooting)
                                {
                                    Shoot();
                                }
                            }
                            else if (endTimer <= 0.0f)
                            {
                                endTimer = -1.0f;
                                gun.CanFire = false;
                                gun.Fire = false;
                                isEndingTurn = true;
                            }
                        }

                        if (isEndingTurn)
                        {
                            EndTurn();
                        }
                    }
                }
            }

        }
        else
        {
            gun.CanFire = false;
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
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
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
        Vector3 cover = FindCoverAdvanced();
        if (cover != null)
        {
            Move(cover);
        }
        endTimer = 0.5f;
    }

    void Move(Vector3 destination)
    {
        if (agent.remainingDistance <= ap)
        {
            moveDestination = destination;
            agent.destination = destination;
        }
    }

    void Shoot()
    {
        Vector3 offset = new Vector3(0.0f, 0.0f, 0.0f);
        gun.transform.LookAt(player.transform.position + offset, Vector3.up);
        gun.Fire = true;
        isShooting = false;
    }

    bool CheckLineOfSight(Vector3 checkPosition)
    {
        LayerMask layerMask = LayerMask.GetMask("Default");
        checkOrigin = checkPosition;
        checkOrigin.y += 1.0f;
        playerDirection = player.transform.position - checkOrigin;

        if (Physics.Raycast(checkOrigin, playerDirection, out hit, Mathf.Infinity))
        {
            if (hit.collider.tag == "Player")
            {
                Debug.Log("Did Hit");
                isShooting = true;
                return true;
            }
        }

        Debug.Log("Did Not Hit");
        isShooting = false;
        return false;
    }

    Vector3 FindCoverSimple() //Finding the nearest reachable cover object
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
                float dist = Vector3.Distance(transform.position, cover.transform.position);

                if (dist < bestCoverDist) //If the distance is the best distance
                {
                    bestCoverDist = dist;
                    bestCover = cover;
                }
            }
        }

        //------------- Then find the furthest cover point on the cover object from the enemy ---------
        if (bestCover != null)
        {
            int bestPointID = -1;
            for (int id = 0; id < bestCover.coverPoints.Length; id++)
            {
                CoverPoint coverPoint = bestCover.coverPoints[id];

                float dist = Vector3.Distance(coverPoint.pos, player.transform.position);

                if (dist > bestPointDist && bestCover.coverPoints[id].isTaken == false)
                {
                    currentCover = bestCover;
                    bestPointID = coverPoint.id;
                    coverPointID = coverPoint.id;
                    bestCover.coverPoints[id].isTaken = true;
                    currentCover.coverPoints[coverPointID].isTaken = true;
                    bestPointDist = dist;
                    bestPoint = coverPoint.pos;
                }
            }
        }

        return bestPoint;
    }

    Vector3 FindCoverAdvanced() //Finding the nearest reachable cover object
    {
        Vector3 initialPoint = transform.position;

        viablePoints = new List<CoverPoint>();

        //------------ Find all cover objects within range
        for (int i = 0; i < coverObjects.Length; i++)
        {
            CoverObject cover = coverObjects[i];

            NavMeshPath path = new NavMeshPath();
            agent.CalculatePath(cover.transform.position, path);

            //If the cover is reachable
            if (agent.pathStatus == NavMeshPathStatus.PathComplete)
            {
                //If within range
                if (Vector3.Distance(transform.position, cover.transform.position) < ap*2.0f)
                {
                    //Find and add all furthest points
                    int furthest = cover.FurthestSide(player.transform.position);

                    for (int id = 0; id < cover.coverPoints.Length; id++)
                    {
                        CoverPoint point = cover.coverPoints[id];
                        if (cover.isTakenList[id] == false)
                        {
                            if (point.side == furthest)
                            {
                                if (CheckLineOfSight(cover.coverPoints[id].pos))
                                {
                                    viablePoints.Add(point);
                                }
                            }
                        }
                    }
              
                }
            }
        }

        if (viablePoints.Count > 0)
        {
            if (currentCover != null)
            {
               currentCover.isTakenList[coverPointID] = false;
            }

            int randomPoint = Random.Range(0, viablePoints.Count);
            coverPointID = viablePoints[randomPoint].id;
            currentCover = viablePoints[randomPoint].parent;
            currentCover.isTakenList[coverPointID] = true;
            return viablePoints[randomPoint].pos;
        }
        else
        {
            return FindCoverSimple();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(gunObject.transform.position, 0.2f);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(checkOrigin, playerDirection.normalized * hit.distance);
    }

}
