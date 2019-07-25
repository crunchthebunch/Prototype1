using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSight : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    public Material laserMaterial;
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.widthMultiplier = 0.01f;
        lineRenderer.material = laserMaterial;
        
        lineRenderer.enabled = true;
        Info = GetComponent<Guns>();
    }

    // Update is called once per frame
    void Update()
    {
        lineRenderer.SetPosition(0, Info.FirePoint[(int)Info.SelectedGun].position);

        RaycastHit hit;


        if (Physics.Raycast(transform.position, player.transform.localEulerAngles.normalized, out hit))
        {
            //if (hit.collider)
            //{
            //    lineRenderer.SetPosition(1, new Vector3(hit.point.x, hit.point.y, hit.point.z));
            //}
            //else
            //{
            //    lineRenderer.SetPosition(1, new Vector3(transform.forward.x, transform.forward.y, transform.forward.z));
            //}

            lineRenderer.SetPosition(1, hit.point);
        }
        
    }
}
