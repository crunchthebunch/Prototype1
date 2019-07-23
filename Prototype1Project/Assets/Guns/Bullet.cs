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
            KillTimer -= Time.deltaTime;
            transform.position += transform.forward * Speed * Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Gun") && !collision.gameObject.CompareTag("Player") && !collision.gameObject.CompareTag("Bullet"))
        {
            Instantiate(BulletHit[(int)BulletType], transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
