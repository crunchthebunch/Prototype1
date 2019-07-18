using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{

    public float HP = 100.0f;
    public int AP = 10;
    public float moveSpeed = 1.0f;
    public float mouseSpeed = 6.0f;
    public Camera mainCamera;
    public float multiplierAP = 1.0f;

    public GameObject attachedGun;

    public float distanceTravelled;
    public Camera playerCam;
    public int lengthOfLineRenderer = 20;
    LayerMask groundLayerMask;
    NavMeshAgent agent;

    GameManager gameManager;

    bool startedMoving;

    [SerializeField] LineRenderer lineRenderer;

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
        lineRenderer.SetPosition(0, new Vector3(transform.position.x, 0.0f, transform.position.z));
        ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (gameManager.initiativeCount == 0)
        {
            if (gameManager.isShooting == false)
            {
                lineRenderer.enabled = true;

                DrawPath(agent.path);

                //print("Distance: " + Vector3.Distance(transform.position, hit.point));
                //print("AP: " + AP);

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

                if (agent.remainingDistance <= agent.stoppingDistance && startedMoving == true)
                {
                    startedMoving = false;

                    AP -= (int)Vector3.Distance(startPoint, endPoint);
                }
            }

            else
            {
                float X = Input.GetAxis("Mouse X") * mouseSpeed;
                float Y = Input.GetAxis("Mouse Y") * mouseSpeed;

                transform.Rotate(0, X, 0);
                attachedGun.transform.Rotate(-Y, 0, 0);


                if (playerCam.transform.eulerAngles.x + (Y) > 80 && playerCam.transform.eulerAngles.x + (Y) < 280)
                { }
                else
                {

                    playerCam.transform.RotateAround(transform.position, playerCam.transform.right, -Y);
                }
            }
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

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

    void DrawPath(NavMeshPath path)
    {
        //if (path.corners.Length < 2)
        //{
        //    return;
        //}

        if (Vector3.Distance(transform.position, endPoint) < AP * multiplierAP)
        {
            lineRenderer.positionCount = path.corners.Length;

            for (var i = 1; i < path.corners.Length; i++)
            {
                lineRenderer.SetPosition(i, path.corners[i]);
            }
        }
    }
}
