using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickupText : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera cameraToLookAt;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(cameraToLookAt.transform);
        transform.rotation = Quaternion.LookRotation(cameraToLookAt.transform.forward);
    }
}
