using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CellController : MonoBehaviour
{
    // =================================================
    // Reference
    // =================================================
    CellSpawnManager _cellManager;

    Rigidbody2D _rb;
    Collider2D _currentArea;

    // =================================================
    // Cell Movement Option
    // =================================================
    [SerializeField] float changeTime = 1f;
    [SerializeField] float rotationTime = 1f;
    [SerializeField] float _speed = 2f;

    Quaternion _targetRotation;

    float _timer = 0;

    // =================================================
    // Cell Life-Cycle Option
    // =================================================
    [SerializeField] float _lifeTime = 15f;

     // =================================================
     // Cell Self Product Option
     // =================================================
    [SerializeField] float _productTime = 10f;

    // =================================================
    // Cell Stat
    // =================================================
    float _radius;

    // =================================================
    // Layer Hash
    // =================================================
    int _hash_cellMedia;

    // =================================================
    // 상태값
    // =================================================
    bool _isSelfProducting = false;

    // =================================================
    // 초기화
    // =================================================
    public void InitCell(CellSpawnManager manager)
    {
        _cellManager = manager;
    }

    // =================================================
    // Set Life Cycle
    // =================================================
    public void SetLifeCycle(float time)
    {
        _lifeTime = time;
    }

    // =================================================
    // Set Prod Time
    // =================================================
    public void SetProdTime(float time)
    {
        _productTime = time;
    }

    // =================================================
    // Cell Life-Cycle
    // =================================================
    IEnumerator CoCellLifeCycle()
    {
        float time = 0;

        float lifeTimeErrorMin = _lifeTime - 0.7f;
        float lifeTimeErrorMax = _lifeTime + 0.7f;

        float setLifeTime = Random.Range(lifeTimeErrorMin, lifeTimeErrorMax);

        while (time < setLifeTime)
        {
            time += Time.deltaTime;

            yield return null;
        }

        // Cell 삭제 파티클 생성
        

        // 삭제 진행
        _cellManager.DeleteCell(this);
    }

    // =================================================
    // Cell Self Product
    // =================================================
    IEnumerator CoSelfProduct()
    {
        while (true)
        {
            float time = 0;

            while (time < _productTime)
            {
                time += Time.deltaTime;

                yield return null;
            }

            GameManager._GM.GainDNAPoint();
        }
    }

    // =================================================
    // Start Self Product
    // =================================================
    public void StartSelfProduct()
    {
        if (!_cellManager.IsUpgraded) return;
        if (_isSelfProducting) return;

        _isSelfProducting = true;

        StartCoroutine(CoSelfProduct());
    }

    // =================================================
    // Cell Movement
    // =================================================
    public void CellMovement()
    {
        _timer += Time.deltaTime;

        if (_timer >= changeTime)
        {
            _timer = 0f;

            float angle = Random.Range(0f, 360f);

            _targetRotation = Quaternion.Euler(0f,0f, angle) * transform.rotation;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, rotationTime * Time.deltaTime);

        Vector2 nextPos = _rb.position + (Vector2)transform.right * _speed * Time.deltaTime;

        _rb.MovePosition(nextPos);
    }

    // =================================================
    // Cell Area Check
    // =================================================
    public void CellAreaCheck()
    {
        Collider2D collider = Physics2D.OverlapPoint(transform.position, _hash_cellMedia);

        if (collider == null)
        {
            _cellManager.DeleteCell(this);
        }

        _currentArea = collider;

        if (_currentArea == null) return;

        Vector2 center = _currentArea.bounds.center;
        float areaRadius = _currentArea.bounds.extents.x;
        float maxDistance = areaRadius - _radius;

        Vector2 dir = _rb.position - center;

        if (dir.magnitude > maxDistance)
        {
            _rb.position = center + dir.normalized * maxDistance;
        }
    }

    // =================================================
    // Awake
    // =================================================
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        if (_rb != null)
        {
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
        }

        _radius = transform.localScale.x / 2f;

        _timer = 0f;

        _targetRotation = transform.rotation;
    }

    // =================================================
    // Start
    // =================================================
    void Start()
    {
        _hash_cellMedia = 1 << LayerMask.NameToLayer("Media");

        StartCoroutine(CoCellLifeCycle());
    }

    void OnDisable()
    {
        _isSelfProducting = false;
    }

    // =================================================
    // FixedUpdate
    // =================================================
    void FixedUpdate()
    {
        CellAreaCheck();
        CellMovement();
    }
}
