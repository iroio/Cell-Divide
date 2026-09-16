using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UpgradesManager : MonoBehaviour
{
    // =================================================
    // Reference
    // =================================================
    [SerializeField] CellSpawnManager _CSManager;

    [SerializeField] TextMeshProUGUI _reduceDelayLevelUI;
    [SerializeField] TextMeshProUGUI _increseDivideAmountLevelUI;
    [SerializeField] TextMeshProUGUI _increseLifeCycleTimeLevelUI;

    CellController _cellController;

    // =================================================
    // Delay Option
    // =================================================
    [Header("Mouse Click Delay Controll Option")]
    [SerializeField] float _maxTime = 10f;
    [SerializeField] float _minTime = 0.5f;
    [SerializeField] int _reduceLevel = 50;

    int _reduceDelayLevel = 0;

    float _decrease;

    // =================================================
    // Divide Option
    // =================================================
    [Header("Cell Divide Amount Option")]
    [SerializeField] int _maxDivideAmount = 50;

    int _increseDivideAmountLevel = 0;

    // =================================================
    // LifeCycle Option
    // =================================================
    [Header("Cell Life-Cycle Increse Option")]
    [SerializeField] float _maxLifeTime = 180f;
    [SerializeField] float _defaultLifeTime = 15f;
    [SerializeField] int _increseLevel = 50;

    int _increseLifeCycleLevel = 0;

    float _currentLifeTime = 15f;
    float _increse;

    public float CurrentLifeTime => _currentLifeTime;

    // =================================================
    // OnclickReduceDelay
    // =================================================
    public void OnclickReduceDelay()
    {
        if (GameManager._GM.Point < 1f)
        {
            Debug.Log("Point 부족");
            return;
        }

        if (_CSManager.ClickDelay <= _minTime)
        {
            Debug.Log("최소 딜레이 도달");
            return;
        }

        float delay = _CSManager.ClickDelay;
        delay = Mathf.Max(_minTime, delay - _decrease);
        _CSManager.ClickDelayReduce(delay);

        // 임시
        float cost = 1f;
        _reduceDelayLevel++;

        GameManager._GM.LoseDNAPoint(cost);
        UIManager._UM.CountUp(_reduceDelayLevelUI, _reduceDelayLevel, _reduceLevel);
    }

    // =================================================
    // OnclickIncreseDivideAmount
    // =================================================
    public void OnclickIncreseDivideAmount()
    {
        if (GameManager._GM.Point < 1f)
        {
            Debug.Log("Point 부족");
            return;
        }

        int divideAmount = _CSManager.DivideAmount;
        divideAmount++;
        _CSManager.IncreseDivideAmount(divideAmount);

        //임시
        float cost = 1f;
        _increseDivideAmountLevel++;

        GameManager._GM.LoseDNAPoint(cost);
        UIManager._UM.CountUp(_increseDivideAmountLevelUI, _increseDivideAmountLevel, _maxDivideAmount);
    }

    // =================================================
    // OnclickIncreseLifeCycleTime
    // =================================================
    public void OnclickIncreseLifeCycleTime()
    {
        if (GameManager._GM.Point < 1f)
        {
            Debug.Log("Point 부족 → return");
            return;
        }

        if (_currentLifeTime >= _maxLifeTime)
        {
            Debug.Log("최대 생명주기 도달");
            return;
        }

        _currentLifeTime = Mathf.Min(_maxLifeTime, _currentLifeTime + _increse);

        // 임시
        int cost = 1;
        _increseLifeCycleLevel++;

        GameManager._GM.LoseDNAPoint(cost);
        UIManager._UM.CountUp(_increseLifeCycleTimeLevelUI, _increseLifeCycleLevel, _increseLevel);
    }

    // =================================================
    // Awake
    // =================================================
    void Awake()
    {
        _currentLifeTime = _defaultLifeTime;
    }

    // =================================================
    // Start
    // =================================================
    void Start()
    {
        _decrease = (_maxTime - _minTime) / _reduceLevel;
        _increse = (_maxLifeTime - _defaultLifeTime) / _increseLevel;
    }

    // =================================================
    // Update
    // =================================================
    void Update()
    {
        
    }
}
