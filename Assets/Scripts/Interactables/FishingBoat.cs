using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingBoat : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            LevelManager.Instance.LoadLevel("NavigationMenu", "CrossFade");
            //Save progress
            //Save score
        }
    }
}
