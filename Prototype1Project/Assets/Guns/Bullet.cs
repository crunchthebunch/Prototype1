using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Speed;
    public float KillTimer;
    public GameObject[] BulletHit;
    public GameObject[] bulletTrail;
    public Guns.E_Guns BulletType;
    private bool Trail = true;
    public int[] damage;


    // Update is called once per frame
    void Update()
    {
        if (Trail)
        {
            Instantiate(bulletTrail[(int)BulletType], transform);
            Trail = false;
        }
        if (KillTimer <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            killTimer -= Time.deltaTime;
            transform.position += transform.forward * speed * Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Gun") && collision.gameObject.CompareTag("Bullet"))
        {
            Instantiate(bulletHit[(int)bulletType], transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

}

