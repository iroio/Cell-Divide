using System.Collections;
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
    // Cell LifeCycle Option
    // =================================================
    [SerializeField] float _lifeTime = 15f;

    public float LifeTime => _lifeTime;

    // =================================================
    // Cell
    // =================================================
    float _radius;

    // =================================================
    // Layer Hash
    // =================================================
    int _hash_cellMedia;

    // =================================================
    // 초기화
    // =================================================
    public void InitCell(CellSpawnManager manager)
    {
        _cellManager = manager;
    }

    // =================================================
    // Set LifeCycle
    // =================================================
    public void SetLifeCycle(float time)
    {
        _lifeTime = time;
    }

    // =================================================
    // Cell LifeCycle
    // =================================================
    IEnumerator CoCellLifeCycle()
    {
        float time = 0;
        float lifeTime = _lifeTime;

        while (time < lifeTime)
        {
            time += Time.deltaTime;

            yield return null;
        }

        // Cell 삭제 파티클 생성
        

        // 삭제 진행
        _cellManager.DeleteCell(this);
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
    // CellAreaheck
    // =================================================
    public void CellAreaheck()
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

    // =================================================
    // FixedUpdate
    // =================================================
    void FixedUpdate()
    {
        CellAreaheck();
        CellMovement();
    }
}
