using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownCamera : MonoBehaviour
{

    private Vector3 offset;
    Transform target;
    public float lateral = 8;
    public float height = 8;
    public float distance = -4;
    public float rotationSpeed = 5;
    public float smoothSpeed = 0.125f;
    public GameObject playerObject;

    Transform tempTarget;
    Quaternion playerRotation;

    bool playerFocused;
    
    void Start()
    {
        // References to player object to start off with
        playerObject = GameObject.FindGameObjectWithTag("Player");
        target = GameObject.Find("Player").GetComponent<Transform>();
        playerFocused = true;

        // Camera offset with respect to target
        offset = new Vector3(target.position.x + lateral, target.position.y + height, target.position.z + distance);
        transform.position = target.position + offset;
        transform.LookAt(target.position);
    }

    
    void Update()
    {
        // Lerping to next target in initiative order
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
        

        // Allow middle-mouse-click camera rotation only when player character is focused on
        if (target.IsChildOf(playerObject.transform))
        {
            
            if (Input.GetMouseButton(2))
            {
                playerRotation = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * rotationSpeed, Vector3.up);
                offset =  playerRotation * offset;
                
            }
            if (playerFocused)
            {
                transform.LookAt(target.position);
            }
            
        }
    }

    // Changes the camera's target which can be accessed by the GameManager
    public void changeTarget(GameObject newTarget)
    {
        target = newTarget.transform;
        
        if (target.IsChildOf(playerObject.transform))
        {
            //transform.LookAt(target.position);
            playerFocused = true;
        }
        else
        {
            playerFocused = false;
        }
    }
}
