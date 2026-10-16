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
    [SerializeField] CellMediaManager _cellMediaManager;
    [SerializeField] ToxinManager _toxinManager;
    [SerializeField] InputActionReference _divide;

    [SerializeField] GameObject _cellPrefab;
    [SerializeField] Transform _cellRoot;

    [SerializeField] GameObject _particlePrefab;
    [SerializeField] Transform _particleRoot;

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
    bool _isUpgradeProdTime = false;
    bool _isUpgradeSelfDivide = false;
    bool _isUpgradeToxinGenerate = false;

    // =================================================
    // Property
    // =================================================
    public float ClickDelay => _clickDelay;
    public int DivideAmount => _divideAmount;
    public bool IsUpgradeProdTime => _isUpgradeProdTime;
    public bool IsUpgradeSelfDivide => _isUpgradeSelfDivide;
    public float SelfDivideTime => _selfDivideTime;
    public bool IsUpgradeToxinGenerate => _isUpgradeToxinGenerate;

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
        int spawnCount = CellDivide();

        if (spawnCount <= 0)
        {
            yield break;
        }

        // 업그레이드 포인트 획득
        for (int i = 0; i < spawnCount; ++i)
        {
            GameManager._GM.GainDNAPoint();
        }

        _canClick = false;

        yield return new WaitForSeconds(_clickDelay);

        _canClick = true;
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
        _isUpgradeProdTime = true;
    }

    // =================================================
    // Check Upgraded Self Divide
    // =================================================
    public void ChkUpgradeSelfDivide()
    {
        _isUpgradeSelfDivide = true;
    }

    // =================================================
    // Check Upgraded Toxin Generate
    // =================================================
    public void ChkUpgradeToxinGenerate()
    {
        _isUpgradeToxinGenerate = true;
    }

    // =================================================
    // Spawn Cell
    // =================================================
    public bool SpawnCell()
    {
        if (_cellMediaManager.ChildCount >= _maxCellCount)
        {
            Debug.Log("최대 Cell 개수 도달");
            return false;
        }

        var cell = _cellControllerPool.Get();

        if (cell == null)
        {
            Debug.Log("Pool에서 Cell을 가져오지 못함");
            return false;
        }

        cell.SetLifeCycle(_upgradesManager.CurrentLifeTime);
        cell.SetProdTime(_upgradesManager.CurrentProdTime);
        cell.SetDivideRate(_upgradesManager.CurrentDivideRate);
        //cell.SetToxinGenRate(_upgradesManager.

        cell.transform.position = _mWorldPos;
        cell.gameObject.SetActive(true);

        // Cell 갯수 업데이트
        _cellMediaManager.ChilldCount();

        cell.StartCellLifeCycle();
        cell.StartSelfProduct();
        cell.StartSelfDivide();
        cell.StartToxinGenerate();

        return true;
    }

    // =================================================
    // Cell Divide
    // =================================================
    public int CellDivide()
    {
        _mPos = Mouse.current.position.ReadValue();
        _mWorldPos = Camera.main.ScreenToWorldPoint(_mPos);

        _hit = Physics2D.OverlapPoint(_mWorldPos, _hash_cellLayer);

        if (_hit == null)
        {
            Debug.Log("클릭 위치에서 Cell Collider를 찾지 못함");
            return 0;
        }

        CellController cell = _hit.GetComponent<CellController>();

        if(cell == null)
        {
            Debug.Log($"{_hit.name}에서 CellController를 찾지 못함");
            return 0;
        }

        int spawnCount = 0;

        // 생성할 수 만큼 성공여부 확인
        for (int i = 1; i <= _divideAmount; ++i)
        {
            if (!SpawnCell())
            {
                break;
            }

            spawnCount++;
        }

        return spawnCount;
    }

    // =================================================
    // Cell Self Divide
    // =================================================
    public void CellSelfDivide(CellController parentCell)
    {
        _mWorldPos = parentCell.transform.position;

        for (int i = 1; i <= _divideAmount; ++i)
        {
            if (!SpawnCell())
            {
                // 하나라도 생성 실패 시 실패 처리
                return;
            }
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
            cell.InitCell(this, _toxinManager);

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

        // 버튼 클릭할 때도 Input System 입력이 발생
        // 현재 마우스의 위치가 UI오브젝트 위에 있는가 판별
        // 결과값은 bool
        if (EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("UI 위 클릭");
            return;
        }

        if (!_canClick)
        {
            Debug.Log("클릭 딜레이 중");
            return;
        }

        // 클릭 딜레이 적용
        StartCoroutine(CoClickDelay());
    }
}
