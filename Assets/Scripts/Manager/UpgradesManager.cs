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
    [SerializeField] TextMeshProUGUI _buyToxinGeneratorUI;

    [Header("Display Upgrade Level")]
    // dec
    [SerializeField] TextMeshProUGUI _decDelayLevelUI;
    [SerializeField] TextMeshProUGUI _decProdTimeLevelUI;
    [SerializeField] TextMeshProUGUI _decSelfDivideTimeUI;
    // inc
    [SerializeField] TextMeshProUGUI _incDivideAmountLevelUI;
    [SerializeField] TextMeshProUGUI _incLifeCycleTimeLevelUI;
    [SerializeField] TextMeshProUGUI _incSelfDivideRateLevelUI;

    [Header("Display Current Stat")]
    [SerializeField] TextMeshProUGUI _delayUI;
    [SerializeField] TextMeshProUGUI _prodTUI;
    [SerializeField] TextMeshProUGUI _amountUI;
    [SerializeField] TextMeshProUGUI _lctUI;
    [SerializeField] TextMeshProUGUI _sdtUI;
    [SerializeField] TextMeshProUGUI _sdrUI;

    // =================================================
    // Buy Option
    // =================================================
    bool _selfProduct = false;
    bool _selfDivide = false;
    bool _toxinGen  =false;

    // =================================================
    // Buy Options Cost
    // =================================================
    [Header("Buy Options Cost")]
    [SerializeField] float _selfProdCost = 1f;
    [SerializeField] float _selfDivideCost = 1f;
    [SerializeField] float _toxinGenCost = 1f;

    // =================================================
    // Decrease Click Delay Option
    // =================================================
    [Header("Mouse Click Delay Controll Option")]
    [SerializeField] float _defaultDelayTime = 10f;
    [SerializeField] float _minDelayTime = 0.5f;
    [SerializeField] int _decDelayLevel = 50;
    [SerializeField] float _decClickDelayCost = 1f;

    int _delayLevel = 0;

    float _decreaseDelay;

    // =================================================
    // Decrease Product Time Option
    // =================================================
    [Header("Cell Self-Product Time Option")]
    [SerializeField] float _defaultProdTime = 10f;
    [SerializeField] float _minProdTime = 0.2f;
    [SerializeField] int _decProdLevel = 50;
    [SerializeField] float _decProdTimeCost = 1f;

    int _productLevel = 0;

    float _currentProdTime = 10f;
    float _decreaseProdTime;

    public float CurrentProdTime => _currentProdTime;

    // =================================================
    // Increse Divide Amount Option
    // =================================================
    [Header("Cell Divide Option")]
    [SerializeField] int _incAmountLevel = 50;
    [SerializeField] float _incDivideAmountCost = 1f;

    int _amountLevel = 0;

    // =================================================
    // Decrease Self Divide Time Option
    // =================================================
    [Header("Seldf Divide Option")]
    [SerializeField] float _defaultSelfDivideTime = 150f;
    [SerializeField] float _minSelfDivideTime = 100f;
    [SerializeField] int _decSelfdivideTimeLevel = 50;
    [SerializeField] float _decSelfDivideTimeCost = 1f;

    int _selfDivideLevel = 0;

    float _decreaseSelfDivideTime;

    // =================================================
    // Increse Self Divide Rate Option
    // =================================================
    [Header("Seldf Divide Rate Option")]
    [SerializeField] float _defaultDivideRate = 0.005f;
    [SerializeField] float _maxDivideRate = 0.05f;
    [SerializeField] int _incSelfDivideRateLevel = 50;
    [SerializeField] float _incDivideRateCost = 1f;

    int _selfDivideRateLevel = 0;

    float _currentDivideRate = 0.005f;
    float _incSelfDivideRate;

    public float CurrentDivideRate => _currentDivideRate;

    // =================================================
    // Increse Life-Cycle Option
    // =================================================
    [Header("Cell Life-Cycle Increse Option")]
    [SerializeField] float _maxLifeTime = 90f;
    [SerializeField] float _defaultLifeTime = 15f;
    [SerializeField] int _incLifeCycleLevel = 50;
    [SerializeField] float _incLifeCycleTimeCost = 1f;

    int _lifeCycleLevel = 0;

    float _currentLifeTime = 15f;
    float _incLifeCycleTime;

    public float CurrentLifeTime => _currentLifeTime;

    // =================================================
    // Onclick Buy Self Product
    // =================================================
    public void OnClickBuySelfProduct()
    {
        // 이미 구매함 팝업
        if (_selfProduct)
        {
            Debug.Log("이미 자가 생산을 구매했습니다.");
            return;
        }

        if (GameManager._GM.Point < 1f)
        {
            Debug.Log("Point 부족");
            return;
        }

        // 임시
        float cost = _selfProdCost;

        // 업그레이드 확인
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
        if (_selfDivide)
        {
            Debug.Log("이미 자가 분열을 구매했습니다.");
            return;
        }

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
        float cost = _selfDivideCost;

        // 업그레이드 확인
        _CSManager.ChkUpgradeSelfDivide();

        GameManager._GM.LoseDNAPoint(cost);
          
        _selfDivide = true;

        UIManager._UM.Purchase(_buySelfDivideUI, _selfDivide);
    }

    // =================================================
    // Onclick Buy Toxin Generator
    // =================================================
    public void OnclickBuyToxinGen()
    {
        // 이미 구매함 팝업
        if (_toxinGen)
        {
            Debug.Log("이미 독소 생성을 구매했습니다.");
            return;
        }

        if (GameManager._GM.Point < 1f)
        {
            Debug.Log("Point 부족");
            return;
        }

        // 임시
        float cost = _toxinGenCost;

        // 업그레이드 확인
        _CSManager.ChkUpgradeToxinGenerate();

        GameManager._GM.LoseDNAPoint(cost);

        _toxinGen = true;

        UIManager._UM.Purchase(_buyToxinGeneratorUI, _toxinGen);
    }


    // =================================================
    // Onclick Decrease Click Delay
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
        float cost = _decClickDelayCost;
        _delayLevel++;

        GameManager._GM.LoseDNAPoint(cost);
        UIManager._UM.CountUp(_decDelayLevelUI, _delayLevel, _decDelayLevel);
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

        if (_amountLevel >= _incAmountLevel)
        {
            Debug.Log("최대 분열량 레벨 도달");
            return;
        }

        int divideAmount = _CSManager.DivideAmount;
        divideAmount++;
        _CSManager.IncDivideAmount(divideAmount);

        //임시
        float cost = _incDivideAmountCost;
        _amountLevel++;

        GameManager._GM.LoseDNAPoint(cost);
        UIManager._UM.CountUp(_incDivideAmountLevelUI, _amountLevel, _incAmountLevel);
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
        float cost = _decProdTimeCost;

        // Decrease product Time
        _currentProdTime = Mathf.Max(_minProdTime, _currentProdTime - _decreaseProdTime);

        _productLevel++;

        GameManager._GM.LoseDNAPoint(cost);
        UIManager._UM.CountUp(_decProdTimeLevelUI, _productLevel, _decProdLevel);
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

        _currentLifeTime = Mathf.Min(_maxLifeTime, _currentLifeTime + _incLifeCycleTime);

        // 임시
        float cost = _incLifeCycleTimeCost;
        _lifeCycleLevel++;

        GameManager._GM.LoseDNAPoint(cost);
        UIManager._UM.CountUp(_incLifeCycleTimeLevelUI, _lifeCycleLevel, _incLifeCycleLevel);
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

        if(_selfDivide == false)
        {
            Debug.Log("자가분열을 먼저 구매해야 합니다.");
            return;
        }

        if (_lifeCycleLevel < 50)
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
        float cost = _decSelfDivideTimeCost;
        _selfDivideLevel++;

        GameManager._GM.LoseDNAPoint(cost);
        UIManager._UM.CountUp(_decSelfDivideTimeUI, _selfDivideLevel, _decSelfdivideTimeLevel);
    }

    // =================================================
    // Onclick Increse Divide Rate
    // =================================================
    public void OnClickDivideRate()
    {
        if (GameManager._GM.Point < 1f)
        {
            Debug.Log("Point 부족 → return");
            return;
        }

        if (_currentDivideRate >= _maxDivideRate)
        {
            Debug.Log("최대 확률 도달");
            return;
        }

        _currentDivideRate = Mathf.Min(_maxDivideRate, _currentDivideRate + _incSelfDivideRate);

        // 임시
        float cost = _incDivideRateCost;
        _selfDivideRateLevel++;

        GameManager._GM.LoseDNAPoint(cost);
        UIManager._UM.CountUp(_incSelfDivideRateLevelUI, _selfDivideRateLevel, _incSelfDivideRateLevel);
    }

    // =================================================
    // SetUI
    // =================================================
    public void SetUI()
    {
        UIManager._UM.CurrentStat(_delayUI, _prodTUI, _amountUI, _lctUI, _sdtUI, _sdrUI,
           _CSManager.ClickDelay, _currentProdTime, _CSManager.DivideAmount, 
           _currentLifeTime, _CSManager.SelfDivideTime, _currentDivideRate);
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
        _incLifeCycleTime = (_maxLifeTime - _defaultLifeTime) / _incLifeCycleLevel;
        _decreaseProdTime = (_defaultProdTime - _minProdTime) / _decProdLevel;
        _decreaseSelfDivideTime = (_defaultSelfDivideTime - _minSelfDivideTime) / _decSelfdivideTimeLevel;
        _incSelfDivideRate = (_maxDivideRate - _defaultDivideRate) / _incSelfDivideRateLevel;
    }

    // =================================================
    // Update
    // =================================================
    void Update()
    {
        SetUI();
    }
}
