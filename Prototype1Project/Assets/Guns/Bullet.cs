using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Speed;
    public float KillTimer;
    public GameObject[] BulletHit;
    public Guns.E_Guns BulletType;

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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Gun" && collision.gameObject.tag != "Player")
        {
            Instantiate(BulletHit[(int)BulletType], transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
