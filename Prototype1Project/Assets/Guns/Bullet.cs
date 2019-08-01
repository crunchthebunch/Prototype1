using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public float killTimer;
    public Mesh[] M_Bullet;
    public GameObject[] bulletHit;
    public GameObject[] bulletTrail;
    public Guns.E_Guns bulletType;
    public int[] damage;

    private void Awake()
    {
        bulletType = transform.parent.parent.GetComponent<Guns>().SelectedGun;
        transform.parent = null;
        GetComponent<MeshFilter>().mesh = M_Bullet[(int)bulletType];
        GetComponent<MeshCollider>().sharedMesh = M_Bullet[(int)bulletType];
        Instantiate(bulletTrail[(int)bulletType], transform);
    }

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
        Instantiate(bulletHit[(int)bulletType], transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

}

