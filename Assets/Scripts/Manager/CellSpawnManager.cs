using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEngine.ParticleSystem;

public class CellSpawnManager : MonoBehaviour
{
    // =================================================
    // Reference
    // =================================================
    [SerializeField] UpgradesManager _upgradesManager;
    [SerializeField] InputActionReference _divide;
    [SerializeField] GameObject _cellPrefab;
    [SerializeField] Transform _CellRoot;

    // =================================================
    // Layer Hash
    // =================================================
    int _hash_cellLayer;

    // =================================================
    // Upgrade Option
    // =================================================
    [SerializeField] float _clickDelay = 10f;
    [SerializeField] int _divideAmount = 1;
    [SerializeField] float _selfDivideTime = 30f;

    // =================================================
    // 마우스 위치 정보
    // =================================================
    Vector2 _mPos;
    Vector2 _mWorldPos = new Vector2(-11f, 0f);

    bool _canClick = true;

    Collider2D _hit;

    // =================================================
    // 파티클 시스템
    // =================================================
    [SerializeField] ParticleSystem _partcleSystem;

    Vector3 particlePos;

    // =================================================
    // 상태값
    // =================================================
    bool _isUpgraded = false;

    // =================================================
    // Property
    // =================================================
    public float ClickDelay => _clickDelay;
    public int DivideAmount => _divideAmount;
    public bool IsUpgraded => _isUpgraded;

    // =================================================
    // Object Pool
    // =================================================
    GameObjectPool<CellController> _cellControllerPool;

    // =================================================
    // ClickDelay
    // =================================================
    IEnumerator CoClickDelay()
    {
        _canClick = false;

        CellDivide();

        yield return new WaitForSeconds(_clickDelay);

        _canClick = true;
    }

    // =================================================
    // Decrease Click Delay
    // =================================================
    public void DecClickDelay(float amount)
    {
        _clickDelay = amount;
    }

    // =================================================
    // Increase Divide Amount
    // =================================================
    public void IncDivideAmount(int amount)
    {
        _divideAmount = amount;
    }

    // =================================================
    // Check Upgraded Prod Time
    // =================================================
    public void ChkUpgradedProdTime()
    {
        _isUpgraded = true;
    }

    // =================================================
    // Spawn Cell
    // =================================================
    public void SpawnCell()
    {
        var cell = _cellControllerPool.Get();

        if (cell == null) return;

        cell.SetLifeCycle(_upgradesManager.CurrentLifeTime);
        cell.SetProdTime(_upgradesManager.CurrentProdTime);

        cell.transform.position = _mWorldPos;
        cell.gameObject.SetActive(true);

        cell.StartSelfProduct();
    }

    // =================================================
    // Cell Divide
    // =================================================
    public void CellDivide()
    {
        _mPos = Mouse.current.position.ReadValue();
        _mWorldPos = Camera.main.ScreenToWorldPoint(_mPos);

        _hit = Physics2D.OverlapPoint(_mWorldPos, _hash_cellLayer);
        if (_hit == null) return; 

        CellController cell = _hit.GetComponent<CellController>();
        if(cell == null) return;

        for (int i = 1; i <= _divideAmount; ++i)
        {
            SpawnCell();
        }
    }

    // =================================================
    // Delete Cell
    // =================================================
    public void DeleteCell(CellController cell)
    {
        cell.gameObject.SetActive(false);
        _cellControllerPool.Set(cell);

        _partcleSystem.gameObject.SetActive(true);
        _partcleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        _partcleSystem.transform.position = cell.transform.position;
        _partcleSystem.Play();
    }

    // =================================================
    // Awake
    // =================================================
    void Awake()
    {
         _cellControllerPool = new GameObjectPool<CellController> (200, () => 
        {
            var obj = Instantiate(_cellPrefab, _CellRoot);
            obj.SetActive(false);
            var cell = obj.GetComponent<CellController>();
            cell.InitCell(this);

            return cell;
        });
    }

    // =================================================
    // Input System Enable
    // =================================================
    void OnEnable()
    {
        _divide.action.Enable();
    }

    // =================================================
    // Input System Disable
    // =================================================
    void OnDisable()
    {
        _divide.action.Disable();
    }

    // =================================================
    // Start
    // =================================================
    void Start()
    {
        _hash_cellLayer = 1 << LayerMask.NameToLayer("Cell");

        SpawnCell();
    }

    // =================================================
    // Update
    // =================================================
    void Update()
    {
        if (!_divide.action.WasPressedThisFrame()) return;

        // 버튼 클릭할 때도 Input System 입력이 발생
        // 현재 마우스의 위치가 UI오브젝트 위에 있는가 판별
        // 결과값은 bool
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (!_canClick) return;

        // 클릭 딜레이 적용
        StartCoroutine(CoClickDelay());

        // 업그레이드 포인트 획득
        for (int i = 1; i <= _divideAmount; ++i)
        {
            GameManager._GM.GainDNAPoint();
        }
    }
}
