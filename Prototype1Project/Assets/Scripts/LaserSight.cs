using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSight : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.widthMultiplier = 0.01f;
       
        lineRenderer.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        lineRenderer.SetPosition(0, new Vector3(transform.position.x, transform.position.y, transform.position.z));

        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            if (hit.collider)
            {
                lineRenderer.SetPosition(1, new Vector3(hit.point.x, hit.point.y, hit.point.z));
            }
            else
            {
                lineRenderer.SetPosition(1, new Vector3(transform.forward.x, transform.forward.y, transform.forward.z));
            }
        }
        
    }
}
