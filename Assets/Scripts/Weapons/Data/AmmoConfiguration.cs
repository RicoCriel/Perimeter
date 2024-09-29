using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AmmoConfiguration", menuName = "Weapons/AmmoConfiguration", order= 5)]
public class AmmoConfiguration : ScriptableObject
{
    public int MaxAmmo = 120;
    public int ClipSize = 30;
    public int AvailableAmmo = 120;
    public int ClipAmmo = 30;

    private void OnEnable()
    {
        AvailableAmmo = MaxAmmo;
    }
    public void Reload()
    {
        int maxReloadAmount = Mathf.Min(ClipSize, AvailableAmmo);
        int availableBulletsInCurrentClip = ClipSize - ClipAmmo;
        int reloadAmount = Mathf.Min(maxReloadAmount, availableBulletsInCurrentClip);

        ClipAmmo = ClipAmmo + reloadAmount;
        AvailableAmmo -= reloadAmount;
    }

    public bool CanReload()
    {
        return ClipAmmo < ClipSize && AvailableAmmo > 0;
    }

}
