using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSight : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    private Guns Info;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.widthMultiplier = 0.01f;
        lineRenderer.enabled = true;
        Info = GetComponent<Guns>();
    }

    // Update is called once per frame
    void Update()
    {
        lineRenderer.SetPosition(0, Info.FirePoint[(int)Info.SelectedGun].position);

        RaycastHit hit;

        if (Physics.Raycast(Info.FirePoint[(int)Info.SelectedGun].position, transform.forward, out hit))
        {
            if (hit.collider)
            {
                lineRenderer.SetPosition(1, hit.point);
            }
            else
            {
                lineRenderer.SetPosition(1, transform.forward);
            }
        }
        
    }
}
