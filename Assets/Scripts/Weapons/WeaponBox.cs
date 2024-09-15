using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class WeaponBox : MonoBehaviour
{
    [Header("Weapon properties")]
    [SerializeField] private GameObject[] _weapons;
    [SerializeField] private int _selectedWeaponIndex;
    [SerializeField] private Transform _weaponPosition;

    [Header("Animation properties")]
    [SerializeField] private Animation _displayAnimation;
    private Animator _animator;
    private Canvas _boxCanvas;
    private float _timer;
    private const float _animationClipLength = 4f;
    private int _animationCycleCounter;
    private int _animationCycleThreshold;

    private bool _shouldOpenBox;
    public bool CanBuyWeapon;

    public bool IsWeaponCycleDone => CanBuyWeapon;
    private const int _weaponBoxPrice = 150;
    public int WeaponBoxPrice { get; } = _weaponBoxPrice;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _boxCanvas = GetComponentInChildren<Canvas>(true);
    }

    private void Update()
    {
        HandleWeaponBoxLogic();
    }

    public void Interact()
    {
        _shouldOpenBox = true;
    }

    public void Buy()
    {
        BuyWeapon();
        CanBuyWeapon = false;
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
        CanBuyWeapon = false;
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
        _animator.Play("OpenLid");
    }

    private void CloseLid()
    {
        _animator.Play("CloseLid");
        _displayAnimation.Stop("DisplayWeapon");
    }

    private void StartDisplayAnimation()
    {
        _displayAnimation.Play("DisplayWeapon");
    }

    private void UpdateWeaponDisplayCycle()
    {
        _timer += Time.deltaTime;

        if (_timer < _animationClipLength && _animationCycleCounter < _animationCycleThreshold)
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

    private void ResetDisplayCycle()
    {
        _animationCycleCounter = 0;
        _animationCycleThreshold = 0;
        _timer = 0;
        DisableWeapons();
    }

    private void BuyWeapon()
    {
        Weapon selectedWeapon = _weapons[_selectedWeaponIndex].GetComponent<Weapon>();
        WeaponInventory playerInventory = FindObjectOfType<WeaponInventory>();

        playerInventory.EquipWeaponObject(selectedWeapon.Type);
        CloseLid();
    }

    public void ShowInteractUI()
    {
        // Set initial scale to 0 for the pop-up effect
        _boxCanvas.transform.localScale = Vector3.zero;
        _boxCanvas.gameObject.SetActive(true);

        // Animate the scale from 0 to 1 (pop-up effect)
        _boxCanvas.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);

        //Fade in 
        CanvasGroup canvasGroup = _boxCanvas.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1f, 0.3f);
        }
    }

    public void HideInteractUI()
    {
        // Animate the scale down to 0 (disappear effect)
        _boxCanvas.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack)
            .OnComplete(() => _boxCanvas.gameObject.SetActive(false)); 

        CanvasGroup canvasGroup = _boxCanvas.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.DOFade(0f, 0.3f);
        }
    }

}
