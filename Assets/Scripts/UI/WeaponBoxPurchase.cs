using UnityEngine;
using UnityEngine.Events;

public class WeaponBoxPurchase : MonoBehaviour
{
    public UnityEvent OnWeaponChosen;
    public UnityEvent OnWeaponDropping;
    private void EnableBuy()
    {
        OnWeaponChosen?.Invoke();
    }

    private void DisableBuy()
    {
        OnWeaponDropping?.Invoke();
    }
}
