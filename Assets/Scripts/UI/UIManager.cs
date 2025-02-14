using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public LootView _lootView;
    public GameObject _lootPanel;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnEnable()
    {
        Loot.OnLootCollected += UpdateLootView;
        //update inventory
    }

    private void OnDisable()
    {
        Loot.OnLootCollected -= UpdateLootView;
    }

    private void UpdateLootView(string lootType)
    {
        _lootPanel.SetActive(true);
        _lootView.UpdateLootText(lootType);
    }
}
