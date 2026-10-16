using System.Collections;
using UnityEngine;

public class ToxinController : MonoBehaviour
{
    // =================================================
    // Reference
    // =================================================
    ToxinManager _toxinManager;

    Rigidbody2D _rb;
    Collider2D _currentArea;

    // =================================================
    // Toxin Movement Option
    // =================================================
    [SerializeField] float _changeTime = 1f;
    [SerializeField] float _rotationTime = 1f;
    [SerializeField] float _speed = 2f;

    Quaternion _targetRotation;

    float _timer = 0;

    // =================================================
    // Toxin Stat
    // =================================================
    float _radius;

    // =================================================
    // Toxin Option
    // =================================================
    [SerializeField] float _lifeTime = 10f;

    // =================================================
    // Layer Hash
    // =================================================
    int _hash_cellMedia;

    // =================================================
    // InitPoison
    // =================================================
    public void InitToxin(ToxinManager manager)
    {
        _toxinManager = manager;
    }

    // =================================================
    // CoToxinLifeCycle
    // =================================================
    IEnumerator CoToxinLifeCycle()
    {
        float time = 0;

        float lifeTimeErrorMin = _lifeTime - 0.8f;
        float lifeTimeErrorMax = _lifeTime + 0.8f;

        float setLifeTime = Random.Range(lifeTimeErrorMin, lifeTimeErrorMax);

        while (time < setLifeTime)
        {
            time += Time.deltaTime;

            yield return null;
        }

        // 삭제 진행
        _toxinManager.DeleteToxin(this);
    }

    // =================================================
    // Start Self Life-Cycle
    // =================================================
    public void StartToxinLifeCycle()
    {
        StartCoroutine(CoToxinLifeCycle());
    }

    // =================================================
    // Toxin Movement
    // =================================================
    public void ToxinMovement()
    {
        _timer += Time.deltaTime;

        if (_timer >= _changeTime)
        {
            _timer = 0f;

            float angle = Random.Range(0f, 360f);

            _targetRotation = Quaternion.Euler(0f, 0f, angle) * transform.rotation;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, _rotationTime * Time.deltaTime);

        Vector2 nextPos = _rb.position + (Vector2)transform.right * _speed * Time.deltaTime;

        _rb.MovePosition(nextPos);
    }

    // =================================================
    // Toxin Area Check
    // =================================================
    public void ToxinAreaCheck()
    {
        Collider2D collider = Physics2D.OverlapPoint(transform.position, _hash_cellMedia);

        if (collider == null)
        {
            _toxinManager.DeleteToxin(this);
            return;
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
    }

    // =================================================
    // FixedUpdate
    // =================================================
    void FixedUpdate()
    {
        ToxinAreaCheck();
        ToxinMovement();
    }
}
