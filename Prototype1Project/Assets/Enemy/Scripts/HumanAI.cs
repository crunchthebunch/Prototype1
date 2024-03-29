﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

public class HumanAI : MonoBehaviour
{
    public int HP;
    public int AP;
    public int speed;
    public int initiative;

    int maxHP;
    int coverPointID; //id of the coverPoint taken on the current cover, -1 if not in cover

    public bool isMyTurn;
    public bool isAggro;
    bool isDead;
    bool isWandering;
    bool isEndingTurn;
    bool isShooting;
    bool isTakingCover;

    float endTimer; //Timer for ending turn (used mostly for actions)

    private Animator animator;
    private Vector3 moveDestination;
    private CoverObject currentCover;
    private CoverObject[] coverObjects;
    public GameObject gunObject;
    public GameObject gunSlot;
    public ParticleSystem alertParticle;
    GameObject player;

    AudioSource audioSource;
    public AudioClip deathSound;
    public AudioClip alertSound;

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

    public enum EnemyType
    {
        soldier,
        assault,
        sniper
    };

    AI state;
    NavMeshAgent agent;
    public EnemyType enemyType;

    void Start()
    {
        isAggro = false;
        isDead = false;
        state = AI.engage;
        isMyTurn = false;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        gameManager = FindObjectOfType<GameManager>();
        moveDestination = transform.position;
        gun = gunObject.GetComponent<Guns>();
        endTimer = -1.0f;
        checkOrigin = transform.position;
        isShooting = false;
        audioSource = GetComponent<AudioSource>();


        switch (enemyType)
        {
            case EnemyType.soldier:
                HP = 10;
                AP = 6;
                gun.GunSwap(Guns.E_Guns.AssaultRifle);
                break;
            case EnemyType.assault:
                HP = 12;
                AP = 5;
                gun.GunSwap(Guns.E_Guns.Shotgun);
                break;
            case EnemyType.sniper:
                HP = 8;
                AP = 5;
                gun.GunSwap(Guns.E_Guns.Sniper);
                break;
        }

        maxHP = HP;
        agent.angularSpeed = 360.0f;
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

    void TakeDamage(Bullet bullet)
    {
        HP -= bullet.damage[(int)bullet.bulletType];

        if (HP <= 0)
        {
            animator.SetBool("isDead",true);
            gun.DetachGun();
            GetComponent<CapsuleCollider>().enabled = false;
            audioSource.clip = deathSound;
            audioSource.Play();
            isDead = true;
            isAggro = false;
        }
        else
        {
            animator.SetBool("isHit", true);
            Invoke("ResetHit", 0.4f);
        }

        if (!isAggro && !isDead)
        {
            isAggro = true;
            audioSource.clip = alertSound;
            audioSource.Play();
            alertParticle.Play();
            CallBackup();
        }
    }

    void Update()
    {
        float playerDist = Vector3.Distance(transform.position, player.transform.position);

        if (!isAggro && playerDist < AP * 3.0f && CheckLineOfSight(transform.position) && !isDead)
        {
            CallBackup();
            audioSource.clip = alertSound;
            audioSource.Play();
            alertParticle.Play();
            isAggro = true;
        }

        //Checking for if it is this AI's turn
        if (!isMyTurn && CheckTurn() == true)
        {
            isMyTurn = true;
            isEndingTurn = false;
            endTimer = -1.0f;
            isShooting = false;
            gun.CanFire = false;
            animator.SetBool("isCrouching", false);


            if (isAggro && HP > 0)
            {
                CombatBehaviour();
            }
            else
            {
                isEndingTurn = true;
            }
        }

        //This AI's turn
        if (isMyTurn)
        {
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
                                animator.SetBool("isMoving", false);
                                endTimer -= Time.deltaTime;

                                if (CheckLineOfSight(transform.position))
                                {
                                    isShooting = true;
                                }                              

                                if (isShooting)
                                {
                                    if (endTimer < 0.8f && endTimer > 0.4f)
                                    {
                                        animator.SetBool("isShooting", true);
                                        Vector3 target = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
                                        Vector3 offset = new Vector3(0.0f, 0.25f, 0.0f);
                                        transform.LookAt(target, Vector3.up);
                                        gun.transform.LookAt(target + offset, Vector3.up);
                                    }
                                    if (endTimer <= 0.4f)
                                    {
                                        Shoot();
                                    }
                                }
                            }
                            else if (endTimer <= 0.0f)
                            {
                                endTimer = -1.0f;
                                gun.CanFire = false;
                                isEndingTurn = true;
                                animator.SetBool("isShooting", false);
                                animator.SetBool("isCrouching", isTakingCover);
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

        foreach (ContactPoint contact in collision.contacts)
        {
            if (collision.gameObject.tag == "Bullet")
            {
                Debug.DrawRay(contact.point, contact.normal, Color.white);
                Bullet bullet = collision.gameObject.GetComponent<Bullet>();
                TakeDamage(bullet);
            }
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

    int AssessRisk() //Assessing risk
    {
        if (HP < maxHP*0.5f && HP > maxHP*0.25f)
        {
            return 1;
        }
        else if(HP <= maxHP * 0.25f)
        {
            return 2;
        }

        return 0;
    }

    void Engage() //Engaging with enemies
    {
        if (CheckLineOfSight(transform.position))
        {
            isShooting = true;
        }

        if (!isShooting)
        {
            float dist = Vector3.Distance(transform.position, player.transform.position);
            if ( dist < AP * 2)
            {
                Vector3 cover = FindCoverAdvanced();

                if (cover != null)
                {
                    Move(cover);
                }
                endTimer = 0.8f;
                isTakingCover = true;
            }
            else
            {
                Vector3 charge = player.transform.position - transform.position;
                Vector3 offset = Random.insideUnitSphere * 0.1f;

                charge = transform.position + (charge.normalized * AP * 1.5f);
                charge += offset;
              

                Move(charge);
                endTimer = 0.8f;
                isTakingCover = false;
            }
        }
        else
        {
            endTimer = 0.8f;
        }
    }

    void CallBackup()
    {
       for (int i = 0; i < gameManager.enemies.Length; i++)
       {
            HumanAI ally = gameManager.enemies[i].GetComponent<HumanAI>();

            float allyDist = Vector3.Distance(transform.position, ally.transform.position);

            if (allyDist < 10 && ally.isAggro == false && ally.isDead == false)
            {
                ally.audioSource.clip = ally.alertSound;
                ally.audioSource.Play();
                ally.alertParticle.Play();
                ally.isAggro = true;
            }
        }
    }

    void Move(Vector3 destination)
    {
        moveDestination = destination;
        agent.destination = destination;
        animator.SetBool("isMoving", true);
    }

    void Shoot()
    {
        gun.CurrentMag = gun.MaxMagSize[(int)gun.SelectedGun];
        gun.CanFire = true;
        gun.FireGun();
        //isShooting = false;
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
                //isShooting = true;
                return true;
            }
        }

        //Debug.Log("Did Not Hit");
        //isShooting = false;
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
                if (Vector3.Distance(transform.position, cover.transform.position) < AP*2.0f)
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
                                    isShooting = true;
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

    //private void OnDrawGizmos()
    //{
        //Gizmos.color = Color.green;
        //Gizmos.DrawSphere(gunObject.transform.position, 0.2f);
        //Gizmos.color = Color.blue;
        //Gizmos.DrawRay(checkOrigin, playerDirection.normalized * hit.distance);
        ////Handles.color = Color.blue;
        ////Handles.DrawWireDisc(transform.position, transform.up, AP * 3);
    //}

    void ResetHit()
    {
        animator.SetBool("isHit", false);
    }
}
