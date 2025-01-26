using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Island : MonoBehaviour
{
    [SerializeField] private string _islandScene;
    private MeshRenderer _renderer;
    private Color _selectedColor = Color.red;
    private Color _color;

    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        _color = _renderer.material.color;
    }

    public void DisplayIslandInfo()
    {
        _renderer.material.color = _selectedColor;
    }

    public void HideIslandInfo()
    {
        _renderer.material.color = _color;
    }

    public void LoadIsland()
    {
        LevelManager.Instance.LoadLevel(_islandScene,"CrossFade");
    }
}
