using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public ItemInfo itemInfo;
    public GameObject itemGameObject;

    public abstract void Use();
    public abstract void ReloadGun();
    public abstract void Aim();
    public abstract void UnAim();
    public abstract bool IsGunAuto();
}
