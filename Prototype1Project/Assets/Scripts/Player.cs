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

    public int AP = 10;
    public int lengthOfLineRenderer = 20;

    public float HP = 100.0f;
    public float moveSpeed = 1.0f;
    public float mouseSpeed = 6.0f;
    public float multiplierAP = 1.0f;
    public float distanceTravelled;
    private float elapsed = 0.0f;
    private float viewRange = 60.0f;
    float rotVertical;

    public bool startedMoving;

    Vector3 startPoint, endPoint;
    Ray ray;
    RaycastHit hit;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        agent = GetComponent<NavMeshAgent>();
        groundLayerMask = LayerMask.GetMask("Ground");
        mainCamera = Camera.main;
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.widthMultiplier = 0.2f;
        playerCam = GetComponentInChildren<Camera>();
        playerCam.gameObject.SetActive(false);
    }

    void Update()
    {
        // Initializing LineRenderer and Raycasting
        lineRenderer.SetPosition(0, new Vector3(transform.position.x, 0.0f, transform.position.z));
        ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (gameManager.initiativeCount == 0)
        {
            if (gameManager.playerState == GameManager.PlayerState.MOVING)
            {
                attachedGun.CanFire = false;

                // Unlock cursor
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                // Path drawing
                lineRenderer.enabled = true;
                DrawPath(agent.path);

                // Update AP with movement
                if (startedMoving)
                {
                    AP = (int)Vector3.Distance(transform.position, endPoint);
                }

                // Raycasting from camera to ground
                if (Physics.Raycast(ray, out hit, 100.0f, groundLayerMask))
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        agent.isStopped = false;

                        startedMoving = true;
                    }
                    else
                    {
                        getPath();
                    }
                }

                // Player has reached target position, stops
                if (agent.remainingDistance <= agent.stoppingDistance && startedMoving == true)
                {
                    startedMoving = false;
                }
            }

            else if (gameManager.playerState == GameManager.PlayerState.SHOOTING)
            {
                attachedGun.CanFire = true;

                // Constant shooting mode AP drain
                elapsed += Time.deltaTime;
                if (elapsed >= 3f)
                {
                    elapsed = elapsed % 3f;
                    ShootingModeAPDrain();
                }

                // Lock Cursor
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                // Rotate gun with mouse
                float X = Input.GetAxis("Mouse X") * mouseSpeed;
                float Y = Input.GetAxis("Mouse Y") * mouseSpeed;

                Y = Mathf.Clamp(Y, -60.0f, 60.0f);

                transform.Rotate(0, X, 0);
                attachedGun.transform.Rotate(-Y, 0, 0);


                // Rotate camera with mouse
                if (playerCam.transform.eulerAngles.x + (Y) > 80 && playerCam.transform.eulerAngles.x + (Y) < 280)
                {
                 
                }
                else
                {
                    playerCam.transform.RotateAround(transform.position, playerCam.transform.right, -Y);
                }



                // Firing gun
                if (Input.GetMouseButtonDown(0))
                {
                    // Checks for AP and type of gun used, fires and updates AP depending on gun used
                    switch(attachedGun.SelectedGun)
                    {
                        case Guns.E_Guns.Sniper:

                            if (AP >= 3)
                            {
                                attachedGun.Fire = true;
                                AP -= 3;
                            }
                            break;

                        case Guns.E_Guns.AssaltRifle:

                            if (AP >= 2)
                            {
                                attachedGun.Fire = true;
                                AP -= 2;
                            }
                            break;

                        case Guns.E_Guns.ShotGun:

                            if (AP >= 1)
                            {
                                attachedGun.Fire = true;
                                AP -= 1;
                            }
                            break;
                    }
                }
                else
                {
                    attachedGun.Fire = false;
                }
            }
        }

        // Disable showing of path if moving, shooting mode, etc.
        else
        {
            lineRenderer.enabled = false;
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
    void ShootingModeAPDrain()
    {
        if (AP > 0)
        {
            AP = AP - 1;
        }
       
    }
}
