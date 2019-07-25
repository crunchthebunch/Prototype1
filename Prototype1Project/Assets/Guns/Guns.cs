using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Guns : MonoBehaviour
{
    public GameObject Bullet;
    public GameObject Lazor;

    public bool UIGun;
    public TextMeshProUGUI AmmoText;
    public Image GunImage;
    public Sprite[] GunSprites;
    public Transform SnapPoint;

    public bool Fire;
    public bool CanFire;
    public bool Attached;

    public Mesh[] M_guns;
    public Material[] T_guns;
    public Transform[] FirePoint;
    public GameObject[] MuzzleFlash;

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

    public LineRenderer laserSight;

    GameManager gameManager;

    public enum E_Guns
    {
        Sniper = 0,
        AssaultRifle = 1,
        Shotgun = 2,
    }

    public E_Guns SelectedGun;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        laserSight = GetComponentInChildren<LineRenderer>();
        laserSight.enabled = true;
        GunSwap(this);
        AttachGun(SnapPoint);
    }

    // Update is called once per frame
    void Update()
    {

        GunFire(CanFire);
    }

    void GunFire(bool GunCanFire)
    {
        if (GunCanFire)
        {
            if (FireRateTimer > 0)
            {
                FireRateTimer -= Time.deltaTime;
            }
            else
            {
                if (Fire && CurrentMag > 0)
                {
                    Vector3 shootDirection;
                    if (SelectedGun == E_Guns.Shotgun)
                    {
                        for (int i = 20; i > 0; i--)
                        {
                            shootDirection = transform.localRotation.eulerAngles;
                            shootDirection.x += Random.Range(-spreadFactor[(int)SelectedGun], spreadFactor[(int)SelectedGun]);
                            shootDirection.y += Random.Range(-spreadFactor[(int)SelectedGun], spreadFactor[(int)SelectedGun]);
                            Instantiate(Bullet, FirePoint[(int)SelectedGun].position, Quaternion.Euler(shootDirection), FirePoint[(int)SelectedGun]).GetComponent<Bullet>().bulletType = SelectedGun;
                        }
                    }
                    else
                    {
                        shootDirection = FirePoint[(int)SelectedGun].rotation.eulerAngles;
                        shootDirection.x += Random.Range(-spreadFactor[(int)SelectedGun], spreadFactor[(int)SelectedGun]);
                        shootDirection.y += Random.Range(-spreadFactor[(int)SelectedGun], spreadFactor[(int)SelectedGun]);
                        Instantiate(Bullet, FirePoint[(int)SelectedGun].position, Quaternion.Euler(shootDirection), FirePoint[(int)SelectedGun]).GetComponent<Bullet>().bulletType = SelectedGun;
                    }
                    Instantiate(MuzzleFlash[(int)SelectedGun], FirePoint[(int)SelectedGun].position, transform.rotation, FirePoint[(int)SelectedGun]);
                    CurrentMag--;
                    FireRateTimer = FireRate[(int)SelectedGun];
                    if (UIGun)
                    {
                        AmmoText.text = CurrentMag + "/" + MaxMagSize[(int)SelectedGun];
                    }
                }
            }
            Fire = false;
        }
    }

    public void AttachGun(Transform Attachment)
    {
        if (Attachment)
        {
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().detectCollisions = false;
            GetComponent<Rigidbody>().useGravity = false;
            transform.position = Attachment.position;
            transform.parent = Attachment;
        }
    }

    public void GunSwap(Guns swapGun)
    {
        if (swapGun)
        {
            CurrentMag = swapGun.CurrentMag;
            SelectedGun = swapGun.SelectedGun;
        }
        Lazor.transform.position = FirePoint[(int)SelectedGun].position;
        GetComponent<MeshFilter>().mesh = M_guns[(int)SelectedGun];
        GetComponent<MeshCollider>().sharedMesh = M_guns[(int)SelectedGun];
        GetComponent<MeshRenderer>().material = T_guns[(int)SelectedGun];
        CurrentMag = MaxMagSize[(int)SelectedGun];
        if (UIGun)
        {
            GunImage.sprite = GunSprites[(int)SelectedGun];
            AmmoText.text = CurrentMag + "/" + MaxMagSize[(int)SelectedGun];
        }
    }
}