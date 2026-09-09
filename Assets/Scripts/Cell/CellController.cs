using Unity.AppUI.Core;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CellController : MonoBehaviour
{
    // =================================================
    // Reference
    // =================================================
    [SerializeField] Transform _cellMedia;

    CellSpawnManager _cellManager;

    Rigidbody2D _rb;

    // =================================================
    // Cell Movement Option
    // =================================================
    [SerializeField] float changeTime = 1f;
    [SerializeField] float rotationTime = 1f;
    [SerializeField] float _speed = 2f;

    Quaternion _targetRotation;

    float _timer = 0;
    
    // =================================================
    // Cell
    // =================================================
    float _radius;

    // =================================================
    // 상태값
    // =================================================
    bool _isExit = false;

    // =================================================
    // 초기화
    // =================================================
    public void InitCell(CellSpawnManager manager)
    {
        _cellManager = manager;
    }

    // =================================================
    // 세포 기본 움직임
    // =================================================
    public void CellMovement()
    {
        _timer += Time.deltaTime;

        if (_timer >= changeTime)
        {
            _timer = 0f;

            float angle = Random.Range(-90f, 90f);

            _targetRotation = Quaternion.Euler(0f,0f, angle) * transform.rotation;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, rotationTime * Time.deltaTime);

        Vector2 nextPos = _rb.position + (Vector2)transform.right * _speed * Time.deltaTime;

        _rb.MovePosition(nextPos);
    }

    public void ExitCheck()
    {

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
    // Update
    // =================================================
    void Update()
    {
        CellMovement();
    }
}
