using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    public float HP = 100.0f;
    public int AP = 10;
    public float moveSpeed = 1.0f;
    public Camera mainCamera;

    LayerMask groundLayerMask;
    NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        groundLayerMask = LayerMask.GetMask("Ground");
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {


        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100.0f, groundLayerMask))
            { 
                agent.destination = hit.point;
            }
        }
    }
}
