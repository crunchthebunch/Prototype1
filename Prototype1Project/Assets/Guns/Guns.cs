using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Guns : MonoBehaviour
{
    public GameObject Bullet;
    public Text AmmoText;

    public bool Fire;
    public bool CanFire;

    public Mesh[] M_guns;
    public Transform[] FirePoint;

    [Header("Gun Propertys")]
    public float[] spreadFactor;
    public float[] Damage;

    public int[] MaxMagSize;
    public int CurrentMag;

    //Not sure on these
    public float[] Reload;
    public float ReloadTimer;

    //Pretty sure on these
    public float[] FireRate;
    public float FireRateTimer;

    public enum E_Guns
    {
        Sniper = 0,
        AssaltRifle = 1,
        ShotGun = 2,
    }

    public E_Guns SelectedGun;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MeshFilter>().mesh = M_guns[(int)SelectedGun];
        CurrentMag = MaxMagSize[(int)SelectedGun];
        AmmoText.text = "Ammo: " + CurrentMag + "/" + MaxMagSize[(int)SelectedGun];
    }

    // Update is called once per frame
    void Update()
    {
        if (CanFire)
        {
            if (FireRateTimer > 0)
            {
                FireRateTimer -= Time.deltaTime;
            }
            else
            {
                if ((Input.GetMouseButton(0) || Fire) && CurrentMag > 0)
                {
                    Vector3 shootDirection;
                    if (SelectedGun == E_Guns.ShotGun)
                    {
                        for (int i = 20; i > 0; i--)
                        {
                            shootDirection = transform.rotation.eulerAngles;
                            shootDirection.x += Random.Range(-spreadFactor[(int)SelectedGun], spreadFactor[(int)SelectedGun]);
                            shootDirection.y += Random.Range(-spreadFactor[(int)SelectedGun], spreadFactor[(int)SelectedGun]);
                            Instantiate(Bullet, FirePoint[(int)SelectedGun].position, Quaternion.Euler(shootDirection));
                        }
                    }
                    else
                    {
                        shootDirection = transform.rotation.eulerAngles;
                        shootDirection.x += Random.Range(-spreadFactor[(int)SelectedGun], spreadFactor[(int)SelectedGun]);
                        shootDirection.y += Random.Range(-spreadFactor[(int)SelectedGun], spreadFactor[(int)SelectedGun]);
                        Instantiate(Bullet, FirePoint[(int)SelectedGun].position, Quaternion.Euler(shootDirection));
                    }
                    CurrentMag--;
                    Fire = false;
                    FireRateTimer = FireRate[(int)SelectedGun];
                    AmmoText.text = "Ammo: " + CurrentMag + "/" + MaxMagSize[(int)SelectedGun];
                }
            }
        }
    }
}
