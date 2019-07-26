using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpIndicator : MonoBehaviour
{
    public Material canPickUp;
    public Material cantPickUp;

    public float rotationSpeed;
    private float num;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        num += Time.deltaTime * rotationSpeed;
        Vector3 temp = Vector3.up;
        temp.y = transform.rotation.y + num;
        transform.rotation = Quaternion.Euler(temp);
    }
}
