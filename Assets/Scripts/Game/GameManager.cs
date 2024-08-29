using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public List<GameObject> activeChickens = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void AddChicken(GameObject chicken)
    {
        activeChickens.Add(chicken);
    }

    public void RemoveChicken(GameObject chicken)
    {
        activeChickens.Remove(chicken);
    }

    public GameObject GetRandomChicken()
    {
        if (activeChickens.Count == 0)
            return null;

        int randomIndex = Random.Range(0, activeChickens.Count);
        return activeChickens[randomIndex];
    }
}
