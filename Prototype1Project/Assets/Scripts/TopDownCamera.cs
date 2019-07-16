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

    Transform tempTarget;
    bool movingPosition;

    // Start is called before the first frame update
    void Start()
    {
        movingPosition = false;
        target = GameObject.Find("Player").GetComponent<Transform>();

        offset = new Vector3(target.position.x + lateral, target.position.y + height, target.position.z + distance);
        transform.position = target.position + offset;
        transform.LookAt(target.position);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        if (Input.GetMouseButton(2))
        {
            offset = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * rotationSpeed, Vector3.up) * offset;
        }

    }

    public void changeTarget(GameObject newTarget)
    {
        target = newTarget.transform;
    }
}
