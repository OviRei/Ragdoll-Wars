using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class AutoGun : Gun
{
    //AR, SMG

    [SerializeField] private PhotonView PV;
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask gunIgnoreLayers;
    [SerializeField] private int currentAmmo;
    [SerializeField] private float nextTimeToFire = 0f;
    [SerializeField] private float _fireRate;
    [SerializeField] private TextMeshProUGUI ammoText;
    private bool isReloading;
    public bool canShoot;
    [SerializeField] private Image crosshair;
    [SerializeField] private PlayerLook playerLook;
    [SerializeField] private Recoil recoil;

    private bool aim;
    private bool canAim = true;

    private void Start()
    {
        if(!PV.IsMine) return;

        _fireRate = ((GunInfo)itemInfo).fireRate;
        currentAmmo = ((GunInfo)itemInfo).magSize;
    }

    private void Update()
    {
        if(!PV.IsMine) return;

        ammoText.text = $"{currentAmmo}/{((GunInfo)itemInfo).magSize}";

        if(aim) cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 40, 15 * Time.deltaTime);
        else cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 70, 15 * Time.deltaTime);

        if(currentAmmo <= 0) ReloadGun();
    }

    public override void Use()
    {
        if(Time.time >= nextTimeToFire && currentAmmo > 0 && !isReloading)
        {
            nextTimeToFire = Time.time + 1f / _fireRate;
            Shoot();
        }
    }

    public override void ReloadGun()
    {
        if(isReloading || currentAmmo >= ((GunInfo)itemInfo).magSize) return;
        canAim = false;
        StartCoroutine(Reload());
    }

    public override void Aim()
    {
        aim = true;
        playerLook.sensX = playerLook.sensX / 2.5f;
        playerLook.sensY = playerLook.sensY / 2.5f;
        //crosshair.enabled = false;    
        gameObject.GetComponent<Animator>().Play("Aim");
    }

    public override void UnAim()
    {
        aim = false;
        playerLook.sensX = playerLook.sensX * 2.5f;
        playerLook.sensY = playerLook.sensY * 2.5f;
        gameObject.GetComponent<Animator>().Play("UnAim");
        //crosshair.enabled = true;
    }

    public override bool IsGunAuto()
    {
        return ((GunInfo)itemInfo).isAutomatic;
    }
    
    private void Shoot()
    {
        if(!PV.IsMine) return;
        
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = cam.transform.position;

        currentAmmo--;

        if(Physics.SphereCast(ray, 0.35f, out RaycastHit hit, 1000f, ~gunIgnoreLayers, QueryTriggerInteraction.Ignore))
        {
            if(hit.collider.tag == "UnShootable") return;
            else if(hit.collider.tag == "Head") hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).headShotDamage);
            else if(hit.collider.tag == "Body") hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).baseDamage);
        }
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        gameObject.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Animator>().Play("UnInsertMag");

        yield return new WaitForSeconds(((GunInfo)itemInfo).reloadTime);

        gameObject.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Animator>().Play("InsertMag");
        currentAmmo = ((GunInfo)itemInfo).magSize;
        isReloading = false;
        canAim = true;
    }
}
