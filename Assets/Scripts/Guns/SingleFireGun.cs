using UnityEngine;
using Photon.Pun;

public class SingleFireGun : Gun
{
    //Hand Cannon, Sniper

    [SerializeField] private PhotonView PV;
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask gunIgnoreLayers;

    public override void Use()
    {
        Shoot();
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

        if(Physics.SphereCast(ray, 0.35f, out RaycastHit hit, 1000f, ~gunIgnoreLayers, QueryTriggerInteraction.Ignore))
        {
            if(hit.collider.tag == "UnShootable") return;
            else if(hit.collider.tag == "Head") hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).headShotDamage);
            else if(hit.collider.tag == "Body") hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).baseDamage);
        }
    }

    public override void Aim()
    {
        gameObject.GetComponent<Animator>().Play("Aim");
        //crosshair.enabled = false;    
    }

    public override void UnAim()
    {
        gameObject.GetComponent<Animator>().Play("UnAim");
        //crosshair.enabled = true;
    }

    public override void ReloadGun()
    {
        //if(isReloading || currentAmmo >= 0) return;
        //StartCoroutine(Reload());
    }
}