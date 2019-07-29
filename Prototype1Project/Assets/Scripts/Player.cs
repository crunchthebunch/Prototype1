using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    public Camera mainCamera;
    public Camera playerCam;
    public Guns attachedGun;
    GameManager gameManager;
    NavMeshAgent agent;
    LayerMask groundLayerMask;
    [SerializeField] LineRenderer lineRenderer;
    public Animator animator;

    public int AP = 10;
    public int lengthOfLineRenderer = 20;
    int distanceSelected = 0;

    public float HP = 100.0f;
    public float moveSpeed = 1.0f;
    public float mouseSpeed = 6.0f;
    public float multiplierAP = 1.0f;
    public float distanceTravelled;
    private float elapsed = 0.0f;
    private float viewRange = 60.0f;
    float rotVertical;
    float X, Y;

    public bool startedMoving;
    bool isDead;
    bool updatingAP;
    bool isCrouching;

    Vector3 startPoint, endPoint;
    public GameObject defaultCam;
    public GameObject crouchCam;
    Ray ray;
    RaycastHit hit;

    void Start()
    {
        isDead = false;
        gameManager = FindObjectOfType<GameManager>();
        agent = GetComponent<NavMeshAgent>();
        agent.angularSpeed = 360.0f;
        groundLayerMask = LayerMask.GetMask("Ground");
        mainCamera = Camera.main;
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.widthMultiplier = 0.2f;
        playerCam = GetComponentInChildren<Camera>();
        playerCam.gameObject.SetActive(false);
        updatingAP = false;
        isCrouching = false;
        animator.speed = 0.75f;
        attachedGun.GunSetUp();
    }

    void Update()
    {
        if (!isDead)
        {
            // Initializing LineRenderer and Raycasting
            lineRenderer.SetPosition(0, new Vector3(transform.position.x, 0.1f, transform.position.z));
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (isCrouching)
            {
                playerCam.transform.position = crouchCam.transform.position;
            }
            else
            {
                playerCam.transform.position = defaultCam.transform.position;
            }

            if (gameManager.initiativeCount == 0)
            {
                if (gameManager.playerState == GameManager.PlayerState.MOVING)
                {
                    animator.SetBool("isAiming", false);
                    attachedGun.CanFire = false;

                    // Unlock cursor
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;

                    // Path drawing
                    lineRenderer.enabled = true;
                    DrawPath(agent.path);

                    // Update AP with movement


                    // Raycasting from camera to ground
                    if (Physics.Raycast(ray, out hit, 100.0f, groundLayerMask))
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            agent.isStopped = false;

                            animator.SetBool("isMoving", true);

                            startedMoving = true;

                            updatingAP = true;
                        }
                        else
                        {
                            getPath();
                        }
                    }

                    if (startedMoving)
                    {
                        elapsed += Time.deltaTime;
                        if (elapsed >= 0.5f)
                        {
                            elapsed = elapsed % 0.5f;
                            APDrain();
                        }
                    }

                    if (Input.GetKeyDown(KeyCode.C))
                    {
                        if (!isCrouching)
                        {
                            animator.SetBool("isCrouching", true);
                            isCrouching = true;
                            AP -= 1;
                        }
                        else
                        {
                            animator.SetBool("isCrouching", false);
                            isCrouching = false;
                            AP -= 1;
                        }

                    }

                    // Player has reached target position, stops
                    if (agent.remainingDistance <= agent.stoppingDistance && startedMoving == true)
                    {
                        startedMoving = false;
                        animator.SetBool("isMoving", false);
                    }
                }

                else if (gameManager.playerState == GameManager.PlayerState.SHOOTING)
                {
                    animator.SetBool("isAiming", true);
                    attachedGun.CanFire = true;

                    // Constant shooting mode AP drain
                    //elapsed += Time.deltaTime;
                    //if (elapsed >= 3f)
                    //{
                    //    elapsed = elapsed % 3f;
                    //    APDrain();
                    //}

                    // Lock Cursor
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;

                    // Rotate gun with mouse
                    X = Input.GetAxis("Mouse X") * mouseSpeed;
                    Y += Input.GetAxis("Mouse Y") * mouseSpeed;
                    Y = Mathf.Clamp(Y, -45.0f, 45.0f);

                    transform.Rotate(0, X, 0);
                    transform.localEulerAngles = new Vector3(-Y, transform.localEulerAngles.y, 0);

                    // Firing gun
                    if (Input.GetMouseButtonDown(0))
                    {
                        // Checks for AP and type of gun used, fires and updates AP depending on gun used
                        switch (attachedGun.SelectedGun)
                        {
                            case Guns.E_Guns.Sniper:

                                if (AP >= 3)
                                {
                                    attachedGun.Fire = true;
                                    animator.SetBool("isShooting", true);
                                    AP -= 3;
                                }
                                break;

                            case Guns.E_Guns.AssaultRifle:

                                if (AP >= 2)
                                {
                                    attachedGun.Fire = true;
                                    animator.SetBool("isShooting", true);
                                    AP -= 2;
                                    print(AP);
                                }
                                break;

                            case Guns.E_Guns.Shotgun:

                                if (AP >= 1)
                                {
                                    attachedGun.Fire = true;
                                    animator.SetBool("isShooting", true);
                                    AP -= 1;
                                }
                                break;

                            default:

                                break;
                        }
                    }
                    else
                    {
                        animator.SetBool("isShooting", false);
                        attachedGun.Fire = false;
                    }

                    // Crouching
                    if (Input.GetKeyDown(KeyCode.C))
                    {
                        if (!isCrouching)
                        {
                            animator.SetBool("isCrouching", true);
                            isCrouching = true;
                        }
                        else
                        {
                            animator.SetBool("isCrouching", false);
                            isCrouching = false;
                        }

                    }

                    // Switch aiming sides
                    if (Input.GetKeyDown(KeyCode.X))
                    {
                        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                    }
                }
            }

            // Disable showing of path if moving, shooting mode, etc.
            else
            {
                lineRenderer.enabled = false;
            }
        }
    }

    // Calculates and draws path depending on AP
    void getPath()
    {
        if (!startedMoving)
        {
            lineRenderer.SetPosition(0, new Vector3(transform.position.x, 0.0f, transform.position.z));

            startPoint = lineRenderer.GetPosition(0);

            if (Vector3.Distance(transform.position, hit.point) < AP * multiplierAP)
            {
                agent.SetDestination(hit.point);

                endPoint = hit.point;
            }

            DrawPath(agent.path);

            agent.isStopped = true;

           
        }
    }

    // Renders path on ground
    void DrawPath(NavMeshPath path)
    {
        if (Vector3.Distance(transform.position, endPoint) < AP * multiplierAP)
        {
            lineRenderer.positionCount = path.corners.Length;

            for (var i = 1; i < path.corners.Length; i++)
            {
                lineRenderer.SetPosition(i, path.corners[i]);
            }
        }
    }

    // Drains AP by 1
    void APDrain()
    {
        if (AP > 0)
        {
            AP = AP - 1;
        }
       
    }

    void TakeDamage(Bullet bullet)
    {
        if ( HP > 0)
        {
            HP -= bullet.damage[(int)bullet.bulletType];
        }

        if ( HP <= 0)
        {
            isDead = true;
            animator.SetBool("isDead", true);
        }
        else
        {
            animator.SetBool("isHit", true);
            Invoke("ResetHit", 0.4f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Bullet tempBullet = collision.gameObject.GetComponent<Bullet>();
            TakeDamage(tempBullet);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Gun"))
        {
            Guns tempGun = other.GetComponent<Guns>();
            if (Input.GetKeyDown(KeyCode.E))
            {
                attachedGun.GunSwap(tempGun);
            }
        }
    }

    void ResetHit()
    {
        animator.SetBool("isHit", false);
    }
}
