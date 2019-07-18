using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Speed;
    public float KillTimer;

    // Update is called once per frame
    void Update()
    {
        if (KillTimer <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            KillTimer -= Time.deltaTime;
            transform.position += transform.forward * Speed * Time.deltaTime;
        }
    }
}
