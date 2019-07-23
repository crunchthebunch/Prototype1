using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public int[] damage;
    public float killTimer;
    public GameObject[] bulletHit;
    public Guns.E_Guns bulletType;


    // Update is called once per frame
    void Update()
    {
        if (killTimer <= 0)
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

