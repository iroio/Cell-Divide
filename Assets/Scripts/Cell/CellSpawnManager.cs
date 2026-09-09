using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CellSpawnManager : MonoBehaviour
{
    // =================================================
    // Reference
    // =================================================
    [SerializeField] InputActionReference _divide;
    [SerializeField] GameObject _cellPrefab;
    [SerializeField] Transform _CellRoot;

    int _hash_cellLayer;

    // =================================================
    // 마우스 정보
    // =================================================
    [SerializeField] float _ClickDelay = 1f;
    Vector2 _mPos;
    Vector2 _mWorldPos = new Vector2(-11f, 0f);

    bool _canClick = true;

    Collider2D _hit;

    // =================================================
    // Object Pool
    // =================================================
    GameObjectPool<CellController> _cellControllerPool;

    IEnumerator CoClickDelay()
    {
        _canClick = false;

        CellDivide();

        yield return new WaitForSeconds(_ClickDelay);

        _canClick = true;
    }

    // =================================================
    // 세포 생성
    // =================================================
    public void spawnCell()
    {
        var cell = _cellControllerPool.Get();

        // if (cell == null) return;
        if (cell == null)  return;

        cell.transform.position = _mWorldPos;
        cell.gameObject.SetActive(true);
    }

    // =================================================
    // 세포 분열
    // =================================================
    public void CellDivide()
    {
        _mPos = Mouse.current.position.ReadValue();
        _mWorldPos = Camera.main.ScreenToWorldPoint(_mPos);

        _hit = Physics2D.OverlapPoint(_mWorldPos, _hash_cellLayer);
        if (_hit == null) return; 

        CellController cell = _hit.GetComponent<CellController>();
        if(cell == null) return;
        
        spawnCell();
    }

    // =================================================
    // Awake
    // =================================================
    void Awake()
    {
         _cellControllerPool = new GameObjectPool<CellController> (50, () => 
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

        spawnCell();
    }

    // =================================================
    // Update
    // =================================================
    void Update()
    {
        if (!_divide.action.WasPressedThisFrame()) return;
        if (!_canClick) return;

        StartCoroutine(CoClickDelay());
    }
}
