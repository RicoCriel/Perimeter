using UnityEngine;

public class WeaponBox : MonoBehaviour
{
    [SerializeField] private GameObject[] _weapons; 
    [SerializeField] private int _selectedWeaponIndex; 
    [SerializeField] private Transform _weaponPosition; 

    private Animator _animator; 
    private Animation _displayAnimation; 
    private float _displayTimer; 
    private int _animationCycleCounter; 
    private int _animationCycleThreshold; 

    private bool _shouldOpenBox; 

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _displayAnimation = GetComponentInChildren<Animation>();
    }

    private void Update()
    {
        HandleWeaponBoxLogic();
    }

    public void Interact()
    {
        _shouldOpenBox = true;
    }
    private void HandleWeaponBoxLogic()
    {
        if (_shouldOpenBox)
        {
            OpenWeaponBox();
            _shouldOpenBox = false;
        }
        else if (_displayAnimation.isPlaying)
        {
            UpdateWeaponDisplayCycle();
        }
        else
        {
            ResetDisplayCycle();
        }
    }

    private void DisableWeapons()
    {
        for (int i = 0; i < _weapons.Length; i++)
        {
            _weapons[i].SetActive(false);
        }
    }

    private void OpenWeaponBox()
    {
        OpenLid();
        StartDisplayAnimation();
    }

    private void OpenLid()
    {
        _animator.Play("OpenBox");
    }

    private void CloseLid()
    {
        _animator.Play("CloseLid");
    }

    private void StartDisplayAnimation()
    {
        _displayAnimation.Play("DisplayWeapon");
    }

    private void UpdateWeaponDisplayCycle()
    {
        _displayTimer += Time.deltaTime;

        if (_displayTimer < 4f && _animationCycleCounter < _animationCycleThreshold)
        {
            _animationCycleCounter++;
        }
        else if (_animationCycleCounter == _animationCycleThreshold)
        {
            _animationCycleCounter = 0;
            RandomizeSelectedWeapon();
            _animationCycleThreshold++;
        }
    }

    private void ResetDisplayCycle()
    {
        _animationCycleCounter = 0;
        _animationCycleThreshold = 0;
        _displayTimer = 0;
        DisableWeapons();
    }

    private void RandomizeSelectedWeapon()
    {
        int newWeaponIndex;

        // Make sure a new weapon is selected
        do
        {
            newWeaponIndex = Random.Range(0, _weapons.Length);
        } while (newWeaponIndex == _selectedWeaponIndex);

        _selectedWeaponIndex = newWeaponIndex;

        // Deactivate all weapons and activate the selected one
        for (int i = 0; i < _weapons.Length; i++)
        {
            _weapons[i].SetActive(false);
        }

        _weapons[_selectedWeaponIndex].SetActive(true);
    }
}
