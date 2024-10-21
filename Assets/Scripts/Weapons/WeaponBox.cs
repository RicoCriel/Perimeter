using UnityEngine;
using UnityEngine.Events;
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

    public bool OpenBox;
    public bool CanBuyWeapon;
    public static WeaponBox Instance;

    private const int _weaponBoxPrice = 150;
    public int WeaponBoxPrice { get; } = _weaponBoxPrice;
    public bool ShouldInteract;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(Instance);
        }

        Instance = this;
    }

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _boxCanvas = GetComponentInChildren<Canvas>(true);
    }

    private void Update()
    {
        HandleWeaponBoxLogic();
    }

    private void HandleWeaponBoxLogic()
    {
        if (OpenBox)
        {
            OpenWeaponBox();
            OpenBox = false;
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

        do
        {
            newWeaponIndex = Random.Range(0, _weapons.Length);
        } while (newWeaponIndex == _selectedWeaponIndex);

        _selectedWeaponIndex = newWeaponIndex;
        
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

    public void Buy()
    {
        BuyWeapon();
        CanBuyWeapon = false;
    }

    private void BuyWeapon()
    {
        Weapon selectedWeapon = _weapons[_selectedWeaponIndex].GetComponent<Weapon>();
        WeaponInventory.Instance.EquipWeaponObject(selectedWeapon.Type);
        CloseLid();
    }

    public void ShowInteractUI()
    {
        _boxCanvas.transform.localScale = Vector3.zero;
        _boxCanvas.gameObject.SetActive(true);

        _boxCanvas.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);

        CanvasGroup canvasGroup = _boxCanvas.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1f, 0.3f);
        }
    }

    public void HideInteractUI()
    {
        _boxCanvas.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack)
            .OnComplete(() => _boxCanvas.gameObject.SetActive(false));

        CanvasGroup canvasGroup = _boxCanvas.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.DOFade(0f, 0.3f);
        }
    }

    public void EnableWeaponBuy()
    {
        CanBuyWeapon = true;
    }

    public void DisableWeaponBuy()
    {
        CanBuyWeapon = false;
    }

    public void Interact()
    {
        OpenBox = true;
    }
}
