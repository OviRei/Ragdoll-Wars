using UnityEngine;

[CreateAssetMenu(menuName = "FPS/New Gun")]

public class GunInfo : ItemInfo
{
    public float baseDamage;
    public float headShotDamage;
    public float fireRate;
    public bool isAutomatic;
    public float reloadTime;
    public int magSize;
}
