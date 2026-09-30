using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CellSpawnManager : MonoBehaviour
{
    // =================================================
    // Reference
    // =================================================
    [SerializeField] UpgradesManager _upgradesManager;
    [SerializeField] InputActionReference _divide;
    [SerializeField] GameObject _cellPrefab;
    [SerializeField] Transform _cellRoot;
    [SerializeField] GameObject _particlePrefab;
    [SerializeField] Transform _particleRoot;
    [SerializeField] CellMediaManager _cellMediaManager;

    // =================================================
    // Layer Hash
    // =================================================
    int _hash_cellLayer;

    // =================================================
    // Upgrade Option
    // =================================================
    [SerializeField] float _clickDelay = 10f;
    [SerializeField] int _divideAmount = 1;
    [SerializeField] float _selfDivideTime = 150f;
    [SerializeField] int _maxCellCount = 200;

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
    Vector3 _particlePos;

    // =================================================
    // 상태값
    // =================================================
    bool _isUpgradProdTime = false;
    bool _isUpgradSelfDivide = false;

    // =================================================
    // Property
    // =================================================
    public float ClickDelay => _clickDelay;
    public int DivideAmount => _divideAmount;
    public bool IsUpgradeProdTime => _isUpgradProdTime;
    public bool IsUpgradeSelfDivide => _isUpgradSelfDivide;
    public float SelfDivideTime => _selfDivideTime;

    // =================================================
    // Object Pool
    // =================================================
    GameObjectPool<CellController> _cellControllerPool;
    GameObjectPool<ParticleSystem> _particleSystemsPool;

    // =================================================
    // ClickDelay
    // =================================================
    IEnumerator CoClickDelay()
    {
        Debug.Log("========== [6] CoClickDelay 진입 ==========");

        _canClick = false;

        Debug.Log("[7] _canClick = false");

        CellDivide();

        Debug.Log("[8] CellDivide() 종료");

        yield return new WaitForSeconds(_clickDelay);

        _canClick = true;

        Debug.Log("[9] 클릭 딜레이 종료 → _canClick = true");
    }

    // =================================================
    // Return Particle
    // =================================================
    IEnumerator CoReturnParticle(ParticleSystem particle)
    {
        yield return new WaitUntil(() => !particle.IsAlive());

        particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        particle.gameObject.SetActive(false);

        _particleSystemsPool.Set(particle);
    }

    // =================================================
    // Decrease Click Delay
    // =================================================
    public void DecClickDelay(float amount)
    {
        _clickDelay = amount;
    }

    // =================================================
    // Decrease Self Divide Time
    // =================================================
    public void DecSelfDivideTime(float time)
    {
        _selfDivideTime = time;
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
    public void ChkUpgradeProdTime()
    {
        _isUpgradProdTime = true;
    }

    // =================================================
    // Check Upgraded Self Divide
    // =================================================
    public void ChkUpgradeSelfDivide()
    {
        _isUpgradSelfDivide = true;
    }

    // =================================================
    // Spawn Cell
    // =================================================
    public void SpawnCell()
    {
        Debug.Log("========== [19] SpawnCell 시작 ==========");

        Debug.Log($"[20] 현재 Cell 개수 = {_cellMediaManager.ChildCount} / {_maxCellCount}");

        if (_cellMediaManager.ChildCount >= _maxCellCount)
        {
            Debug.Log("[21] X 최대 Cell 개수 도달 → Spawn 취소");
            return;
        }

        Debug.Log("[21] O Cell 개수 제한 통과");

        var cell = _cellControllerPool.Get();

        if (cell == null)
        {
            Debug.Log("[22] X Pool에서 Cell을 가져오지 못함");
            return;
        }

        Debug.Log($"[22] O Pool Cell 획득 : {cell.name}");

        cell.SetLifeCycle(_upgradesManager.CurrentLifeTime);
        cell.SetProdTime(_upgradesManager.CurrentProdTime);
        cell.SetDivideRate(_upgradesManager.CurrentDivideRate);

        Debug.Log(
        $"[23] Cell Stat 적용 | " +
        $"LifeTime = {_upgradesManager.CurrentLifeTime}, " +
        $"ProdTime = {_upgradesManager.CurrentProdTime}, " +
        $"DivideRate = {_upgradesManager.CurrentDivideRate}"
    );

        cell.transform.position = _mWorldPos;
        Debug.Log($"[24] Cell Position 설정 = {cell.transform.position}");

        cell.gameObject.SetActive(true);
        Debug.Log($"[25] O Cell Active = {cell.gameObject.activeSelf}");

        // Cell 갯수 업데이트
        _cellMediaManager.ChilldCount();
        Debug.Log($"[26] 현재 Cell 개수 = {_cellMediaManager.ChildCount}");

        cell.StartCellLifeCycle();
        Debug.Log("[27] LifeCycle 시작");
        cell.StartSelfProduct();
        Debug.Log("[28] SelfProduct 시작");
        cell.StartSelfDivide();
        Debug.Log("[29] SelfDivide 시작");

        Debug.Log("========== [30] SpawnCell 완료 ==========");
    }

    // =================================================
    // Cell Divide
    // =================================================
    public void CellDivide()
    {
        Debug.Log("========== [10] CellDivide 시작 ==========");

        _mPos = Mouse.current.position.ReadValue();
        Debug.Log($"[11] Mouse Screen Position = {_mPos}");

        _mWorldPos = Camera.main.ScreenToWorldPoint(_mPos);
        Debug.Log($"[12] Mouse World Position = {_mWorldPos}");

        _hit = Physics2D.OverlapPoint(_mWorldPos, _hash_cellLayer);
        if (_hit == null)
        {
            Debug.Log("[13] X 클릭 위치에서 Cell Collider를 찾지 못함");
            return;
        }

        Debug.Log($"[13] O Cell Collider 발견 : {_hit.name}");

        CellController cell = _hit.GetComponent<CellController>();
        if(cell == null)
        {
            Debug.Log($"[14] X {_hit.name}에서 CellController를 찾지 못함");
            return;
        }

        Debug.Log($"[14] O CellController 발견 : {cell.name}");

        Debug.Log($"[15] Spawn 시작 → DivideAmount = {_divideAmount}");

        for (int i = 1; i <= _divideAmount; ++i)
        {
            Debug.Log($"[16] SpawnCell() 실행 {i}/{_divideAmount}");

            SpawnCell();

            Debug.Log($"[17] SpawnCell() 종료 {i}/{_divideAmount}");
        }

        Debug.Log("[18] CellDivide 완료");
    }

    // =================================================
    // Cell Self Divide
    // =================================================
    public void CellSelfDivide(CellController parentCell)
    {
        _mWorldPos = parentCell.transform.position;

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
        if(!cell.gameObject.activeSelf) return;

        _particlePos = cell.transform.position;

        // Cell Set()
        cell.gameObject.SetActive(false);
        _cellControllerPool.Set(cell);

        // Cell 갯수 업데이트
        _cellMediaManager.ChilldCount();

        // 삭제시 파티클 실행
        var particle = _particleSystemsPool.Get();

        if(particle == null) return;

        particle.transform.position = _particlePos;
        particle.gameObject.SetActive(true);
        particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        particle.Play();

        StartCoroutine(CoReturnParticle(particle));
    }

    // =================================================
    // Awake
    // =================================================
    void Awake()
    {
         _cellControllerPool = new GameObjectPool<CellController> (200, () => 
        {
            var obj = Instantiate(_cellPrefab, _cellRoot);
            obj.SetActive(false);
            var cell = obj.GetComponent<CellController>();
            cell.InitCell(this);

            return cell;
        });

        _particleSystemsPool = new GameObjectPool<ParticleSystem> (5, () =>
        {
            var obj = Instantiate(_particlePrefab, _particleRoot);
            obj.SetActive(false);

            return obj.GetComponent<ParticleSystem>();
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
        Debug.Log("========== [1] 클릭 입력 감지 ==========");

        // 버튼 클릭할 때도 Input System 입력이 발생
        // 현재 마우스의 위치가 UI오브젝트 위에 있는가 판별
        // 결과값은 bool
        if (EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("[2] UI 위 클릭 → Cell 생성 취소");
            return;
        }

        Debug.Log("[2] UI 위 클릭 아님");

        if (!_canClick)
        {
            Debug.Log("[3] 클릭 딜레이 중 → Cell 생성 취소");
            return;
        }

        Debug.Log("[3] 클릭 가능");

        // 클릭 딜레이 적용
        StartCoroutine(CoClickDelay());

        Debug.Log("[4] CoClickDelay 시작");

        // 업그레이드 포인트 획득
        for (int i = 1; i <= _divideAmount; ++i)
        {
            GameManager._GM.GainDNAPoint();
        }

        Debug.Log("[5] DNA Point 획득");
    }
}
