using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBoxController : MonoBehaviour
{
    private Canvas weaponCanvas;

    private void Start()
    {
        weaponCanvas = GetComponentInChildren<Canvas>();
        weaponCanvas.enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            weaponCanvas.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            weaponCanvas.enabled = false;
        }
    }
}
