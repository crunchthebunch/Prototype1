using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillTimer : MonoBehaviour
{
    public float Killtimer;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        timer = Killtimer;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }
}
