using TMPro;
using UnityEngine;

public class UpgradesManager : MonoBehaviour
{
    // =================================================
    // Reference
    // =================================================
    [SerializeField] CellSpawnManager _CSManager;

    [Header("Display Purchase")]
    [SerializeField] TextMeshProUGUI _buySelfProductUI;
    [SerializeField] TextMeshProUGUI _buySelfDivideUI;

    [Header("Display Upgrade Level")]
    // dec
    [SerializeField] TextMeshProUGUI _decDelayLevelUI;
    [SerializeField] TextMeshProUGUI _decProdTimeLevelUI;
    [SerializeField] TextMeshProUGUI _decSelfDivideTimeUI;
    // inc
    [SerializeField] TextMeshProUGUI _incDivideAmountLevelUI;
    [SerializeField] TextMeshProUGUI _incLifeCycleTimeLevelUI;
 

    [Header("Display Current Stat")]
    [SerializeField] TextMeshProUGUI _delayUI;
    [SerializeField] TextMeshProUGUI _prodTUI;
    [SerializeField] TextMeshProUGUI _amountUI;
    [SerializeField] TextMeshProUGUI _lctUI;
    [SerializeField] TextMeshProUGUI _sdtUI;

    // =================================================
    // Buy Option
    // =================================================
    bool _selfProduct = false;
    bool _selfDivide = false;

    // =================================================
    // Delay Option
    // =================================================
    [Header("Mouse Click Delay Controll Option")]
    [SerializeField] float _defaultDelayTime = 10f;
    [SerializeField] float _minDelayTime = 0.5f;
    [SerializeField] int _decDelayLevel = 50;

    int _delayLevel = 0;

    float _decreaseDelay;

    // =================================================
    // Product Time Option
    // =================================================
    [Header("Cell Self-Product Time Option")]
    [SerializeField] float _defaultProdTime = 10f;
    [SerializeField] float _minProdTime = 0.2f;
    [SerializeField] int _decProdLevel = 50;

    int _productLevel = 0;

    float _currentProdTime = 10f;
    float _decreaseProdTime;

    public float CurrentProdTime => _currentProdTime;

    // =================================================
    // Divide Option
    // =================================================
    [Header("Cell Divide Option")]
    [SerializeField] int _incAmountLevel = 50;

    int _amountLevel = 0;

    // =================================================
    // Self Divide Option
    // =================================================
    [Header("Seldf Divide Option")]
    [SerializeField] float _defaultSelfDivideTime = 150f;
    [SerializeField] float _minSelfDivideTime = 30f;
    [SerializeField] int _decSelfdivideTimeLevel = 50;

    int _selfDivideLevel = 0;

    float _decreaseSelfDivideTime;

    // =================================================
    // Life-Cycle Option
    // =================================================
    [Header("Cell Life-Cycle Increse Option")]
    [SerializeField] float _maxLifeTime = 90f;
    [SerializeField] float _defaultLifeTime = 15f;
    [SerializeField] int _incLifeCycleLevel = 50;

    int _lifeCycleLevel = 0;

    float _currentLifeTime = 15f;
    float _increseLCTime;

    public float CurrentLifeTime => _currentLifeTime;

    // =================================================
    // Onclick Buy Self Product
    // =================================================
    public void OnClickBuySelfProduct()
    {
        // 이미 구매함 팝업
        //

        if (GameManager._GM.Point < 1f)
        {
            Debug.Log("Point 부족");
            return;
        }

        // 임시
        float cost = 1f;

        // 자가 생산 여부 확인
        _CSManager.ChkUpgradeProdTime();

        GameManager._GM.LoseDNAPoint(cost);

        _selfProduct = true;

        UIManager._UM.Purchase(_buySelfProductUI, _selfProduct);
    }

    // =================================================
    // Onclick Buy Self Divide
    // =================================================
    public void OnClickBuySelfDivide()
    {
        // 이미 구매함 팝업
        //

        if (GameManager._GM.Point < 1f)
        {
            Debug.Log("Point 부족");
            return;
        }

        if (_lifeCycleLevel < 50)
        {
            Debug.Log("최대 생명 주기에 먼저 도달해야 합니다.");
            return;
        }

        // 임시
        float cost = 1f;

        // 자가 분열
        _CSManager.ChkUpgradeSelfDivide();

        GameManager._GM.LoseDNAPoint(cost);
          
        _selfDivide = true;

        UIManager._UM.Purchase(_buySelfDivideUI, _selfDivide);
    }

    // =================================================
    // Onclick Decrease Self Divide Time
    // =================================================
    public void OnClickDecSelfDivideTime()
    {
        if (GameManager._GM.Point < 1f)
        {
            Debug.Log("Point 부족");
            return;
        }

        if(_lifeCycleLevel < 50)
        {
            Debug.Log("최대 생명 주기에 먼저 도달해야 합니다.");
            return;
        }

        if (_CSManager.SelfDivideTime <= _minSelfDivideTime)
        {
            Debug.Log("최소 딜레이 도달");
            return;
        }

        float divideTime = _CSManager.SelfDivideTime;
        divideTime = Mathf.Max(_minSelfDivideTime, divideTime - _decreaseSelfDivideTime);
        _CSManager.DecSelfDivideTime(divideTime);

        // 임시
        float cost = 1f;
        _selfDivideLevel++;

        GameManager._GM.LoseDNAPoint(cost);
        UIManager._UM.CountUp(_decSelfDivideTimeUI, _selfDivideLevel, _decSelfdivideTimeLevel);
    }

    // =================================================
    // Onclick Decrease Delay
    // =================================================
    public void OnclickDecClickDelay()
    {
        if (GameManager._GM.Point < 1f)
        {
            Debug.Log("Point 부족");
            return;
        }

        if (_CSManager.ClickDelay <= _minDelayTime)
        {
            Debug.Log("최소 딜레이 도달");
            return;
        }

        float delay = _CSManager.ClickDelay;
        delay = Mathf.Max(_minDelayTime, delay - _decreaseDelay);
        _CSManager.DecClickDelay(delay);

        // 임시
        float cost = 1f;
        _delayLevel++;

        GameManager._GM.LoseDNAPoint(cost);
        UIManager._UM.CountUp(_decDelayLevelUI, _delayLevel, _decDelayLevel);
    }

    // =================================================
    // Onclick Decrease Product Time
    // =================================================
    public void OnclickDecPordTime()
    {
        if (GameManager._GM.Point < 1f)
        {
            Debug.Log("Point 부족");
            return;
        }

        if (_selfProduct == false)
        {
            Debug.Log("자가 생산을 먼저 구매해야 합니다.");
            return;
        }

        if (_currentProdTime <= _minProdTime)
        {
            Debug.Log("최소 생산시간 도달");
            return;
        }

        // 임시
        float cost = 1f;

        // Decrease product Time
        _currentProdTime = Mathf.Max(_minProdTime, _currentProdTime - _decreaseProdTime);

        _productLevel++;

        GameManager._GM.LoseDNAPoint(cost);
        UIManager._UM.CountUp(_decProdTimeLevelUI, _productLevel, _decProdLevel);
    }

    // =================================================
    // Onclick Increse Divide Amount
    // =================================================
    public void OnclickIncDivideAmount()
    {
        if (GameManager._GM.Point < 1f)
        {
            Debug.Log("Point 부족");
            return;
        }

        int divideAmount = _CSManager.DivideAmount;
        divideAmount++;
        _CSManager.IncDivideAmount(divideAmount);

        //임시
        float cost = 1f;
        _amountLevel++;

        GameManager._GM.LoseDNAPoint(cost);
        UIManager._UM.CountUp(_incDivideAmountLevelUI, _amountLevel, _incAmountLevel);
    }

    // =================================================
    // Onclick Increse Life Cycle Time
    // =================================================
    public void OnclickIncLifeCycleTime()
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

        _currentLifeTime = Mathf.Min(_maxLifeTime, _currentLifeTime + _increseLCTime);

        // 임시
        int cost = 1;
        _lifeCycleLevel++;

        GameManager._GM.LoseDNAPoint(cost);
        UIManager._UM.CountUp(_incLifeCycleTimeLevelUI, _lifeCycleLevel, _incLifeCycleLevel);
    }

    public void SetUI()
    {
        UIManager._UM.CurrentStat(_delayUI, _prodTUI, _amountUI, _lctUI, _sdtUI,
           _CSManager.ClickDelay, _currentProdTime, _CSManager.DivideAmount, 
           _currentLifeTime, _CSManager.SelfDivideTime);
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
        _decreaseDelay = (_defaultDelayTime - _minDelayTime) / _decDelayLevel;
        _increseLCTime = (_maxLifeTime - _defaultLifeTime) / _incLifeCycleLevel;
        _decreaseProdTime = (_defaultProdTime - _minProdTime) / _decProdLevel;
        _decreaseSelfDivideTime = (_defaultSelfDivideTime - _minSelfDivideTime) / _decSelfdivideTimeLevel;
    }

    // =================================================
    // Update
    // =================================================
    void Update()
    {
        SetUI();
    }
}
