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
    [SerializeField] Transform _CellRoot;

    // =================================================
    // Layer Hash
    // =================================================
    int _hash_cellLayer;

    // =================================================
    // Upgrade Option
    // =================================================
    [SerializeField] float _ClickDelay = 10f;
    [SerializeField] int _divideAmount = 1;

    // =================================================
    // 마우스 위치 정보
    // =================================================
    Vector2 _mPos;
    Vector2 _mWorldPos = new Vector2(-11f, 0f);

    bool _canClick = true;

    Collider2D _hit;

    // =================================================
    // Property
    // =================================================
    public float ClickDelay => _ClickDelay;
    public int DivideAmount => _divideAmount;

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

        yield return new WaitForSeconds(_ClickDelay);

        _canClick = true;
    }

    // =================================================
    // Click Delay Reduce
    // =================================================
    public void ClickDelayReduce(float amount)
    {
        _ClickDelay = amount;
        Debug.Log(_ClickDelay);
    }

    // =================================================
    // Increse Divide Amount
    // =================================================
    public void IncreseDivideAmount(int amount)
    {
        _divideAmount = amount;
        Debug.Log(_divideAmount);
    }

    // =================================================
    // Spawn Cell
    // =================================================
    public void SpawnCell()
    {
        var cell = _cellControllerPool.Get();

        cell.SetLifeCycle(_upgradesManager.CurrentLifeTime);

        if (cell == null) return;

        cell.transform.position = _mWorldPos;
        cell.gameObject.SetActive(true);
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
        if (EventSystem.current.IsPointerOverGameObject())
            return;
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
