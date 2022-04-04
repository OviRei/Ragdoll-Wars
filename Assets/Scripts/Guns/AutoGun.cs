using System.Collections;
using UnityEngine;
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

    private bool test;

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

        if(Input.GetKeyDown(KeyCode.R) || currentAmmo <= 0)
        {
            if(isReloading) return;
            StartCoroutine(Reload());
        }
    }

    public override void Use()
    {
        if(Time.time >= nextTimeToFire && currentAmmo > 0)
        {
            nextTimeToFire = Time.time + 1f / _fireRate;
            Shoot();
        }
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

        yield return new WaitForSeconds(((GunInfo)itemInfo).reloadTime);

        currentAmmo = ((GunInfo)itemInfo).magSize;
        isReloading = false;
    }
}
