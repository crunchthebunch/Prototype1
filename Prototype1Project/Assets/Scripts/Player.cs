using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    public float HP = 100.0f;
    public int AP = 5;
    public float moveSpeed = 1.0f;
    public Camera mainCamera;

    public int lengthOfLineRenderer = 20;
    LayerMask groundLayerMask;
    NavMeshAgent agent;

    bool startedMoving, isMoving;

    [SerializeField] LineRenderer lineRenderer;

    Vector3 startPoint, endPoint;

    Ray ray;
    RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        isMoving = false;
        groundLayerMask = LayerMask.GetMask("Ground");
        mainCamera = Camera.main;
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.widthMultiplier = 0.2f;
    }

    // Update is called once per frame
    void Update()
    {

        lineRenderer.SetPosition(0, new Vector3(transform.position.x, 0.0f, transform.position.z));
        ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (GameManager.initiativeCount == 0)
        {
            lineRenderer.enabled = true;

            DrawPath(agent.path);

            if (Physics.Raycast(ray, out hit, 100.0f, groundLayerMask))
            {

                if (Input.GetMouseButtonDown(0))
                {
                    agent.isStopped = false;

                    startedMoving = true;
                    //if (Physics.Raycast(ray, out hit, 100.0f, groundLayerMask))
                    //{
                    //    agent.destination = hit.point;
                    //}
                    //agent.destination = lineRenderer.GetPosition(1);
                }
                else
                {
                    getPath();
                }

                //Vector3 position1 = new Vector3(transform.position.x, 0.0f, transform.position.z);
                //Vector3 position2 = hit.point;

                //if (Vector3.Distance(position1, position2) < AP)
                //{
                //    lineRenderer.SetPosition(0, position1);
                //    lineRenderer.SetPosition(1, position2);
                //}


                //print(Vector3.Distance(position1, position2));
            }

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                startedMoving = false;
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

            agent.SetDestination(hit.point);

            endPoint = hit.point;

            DrawPath(agent.path);

            agent.isStopped = true;
        }

    }

    void DrawPath(NavMeshPath path)
    {
        if (path.corners.Length < 2)
        {
            return;
        }

        if (Vector3.Distance(startPoint, endPoint) < AP)
        {
            lineRenderer.positionCount = path.corners.Length;

            for (var i = 1; i < path.corners.Length; i++)
            {
                lineRenderer.SetPosition(i, path.corners[i]);
            }
        }
        
        
    }
}
