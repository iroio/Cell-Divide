using TMPro;
using Unity.Mathematics;
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
    [SerializeField] TextMeshProUGUI _incToxinCostUI;
    [SerializeField] TextMeshProUGUI _incToxinGenRateUI;

    [Header("Display Current Stat")]
    [SerializeField] TextMeshProUGUI _delayUI;
    [SerializeField] TextMeshProUGUI _prodTUI;
    [SerializeField] TextMeshProUGUI _amountUI;
    [SerializeField] TextMeshProUGUI _lctUI;
    [SerializeField] TextMeshProUGUI _sdtUI;
    [SerializeField] TextMeshProUGUI _sdrUI;
    [SerializeField] TextMeshProUGUI _tctUI;
    [SerializeField] TextMeshProUGUI _tgrUI;

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

    float _decDelay;

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
    float _decProdTime;

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
    [Header("Self Divide Option")]
    [SerializeField] float _defaultSelfDivideTime = 150f;
    [SerializeField] float _minSelfDivideTime = 100f;
    [SerializeField] int _decSelfdivideTimeLevel = 50;
    [SerializeField] float _decSelfDivideTimeCost = 1f;

    int _selfDivideLevel = 0;

    float _decSelfDivideTime;

    // =================================================
    // Increse Self Divide Rate Option
    // =================================================
    [Header("Self Divide Rate Option")]
    [SerializeField] float _defaultDivideRate = 0.005f;
    [SerializeField] float _maxDivideRate = 0.05f;
    [SerializeField] int _incSelfDivideRateLevel = 50;
    [SerializeField] float _incDivideRateCost = 1f;

    int _selfDivideRateLevel = 0;

    float _currentDivideRate = 0.005f;
    float _incSelfDivideRate;

    public float CurrentDivideRate => _currentDivideRate;

    // =================================================
    // Increse Cell Life-Cycle Option
    // =================================================
    [Header("Cell Life-Cycle Increse Option")]
    [SerializeField] float _defaultCellLifeTime = 15f;
    [SerializeField] float _maxCellLifeTime = 90f;
    [SerializeField] int _incCellLifeTimeLevel = 50;
    [SerializeField] float _incCellLifeTimeCost = 1f;

    int _lifeCycleLevel = 0;

    float _currentLifeTime = 15f;
    float _incLifeCycleTime;

    public float CurrentLifeTime => _currentLifeTime;

    // =================================================
    // Increse Toxin Cost Option
    // =================================================
    [Header("Increse Toxin Cost Option")]
    [SerializeField] float _defaultToxinCost = 1f;
    [SerializeField] float _maxToxinCost = 10f;
    [SerializeField] int _incToxinCostLevel = 10;
    [SerializeField] float _incToxinCostCost = 1f;

    int _toxinCostLevel = 0;

    float _currentToxinCost = 1f;
    float _incToxinCost;

    public float CurrentToxinCost => _currentToxinCost;

    // =================================================
    // Increse Toxin Generate Rate Option
    // =================================================
    [Header("Increse Toxin Generate Rate Option")]
    [SerializeField] float _defaultToxinGenRate = 0.005f;
    [SerializeField] float _maxToxinGenRate = 0.05f;
    [SerializeField] int _incToxinGenRateLevel = 50;
    [SerializeField] float _incToxinGenRateCost = 1f;

    int _toxinGenRateLevel = 0;

    float _currentToxinGenRate = 0.005f;
    float _incToxinGenRate;

    public float CurrentToxinGenRate => _currentToxinGenRate;

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

        if (GameManager._GM.Point < _selfProdCost)
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

        if (GameManager._GM.Point < _selfDivideCost)
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

        if (GameManager._GM.Point < _toxinGenCost)
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
        if (GameManager._GM.Point < _decClickDelayCost)
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
        delay = Mathf.Max(_minDelayTime, delay - _decDelay);
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
        if (GameManager._GM.Point < _incDivideAmountCost)
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
        if (GameManager._GM.Point < _decProdTimeCost)
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
        _currentProdTime = Mathf.Max(_minProdTime, _currentProdTime - _decProdTime);

        _productLevel++;

        GameManager._GM.LoseDNAPoint(cost);
        UIManager._UM.CountUp(_decProdTimeLevelUI, _productLevel, _decProdLevel);
    }

    // =================================================
    // Onclick Increse Life Cycle Time
    // =================================================
    public void OnclickIncCellLifeTime()
    {
        if (GameManager._GM.Point < _incCellLifeTimeCost)
        {
            Debug.Log("Point 부족 → return");
            return;
        }

        if (_currentLifeTime >= _maxCellLifeTime)
        {
            Debug.Log("최대 생명주기 도달");
            return;
        }

        _currentLifeTime = Mathf.Min(_maxCellLifeTime, _currentLifeTime + _incLifeCycleTime);

        // 임시
        float cost = _incCellLifeTimeCost;
        _lifeCycleLevel++;

        GameManager._GM.LoseDNAPoint(cost);
        UIManager._UM.CountUp(_incLifeCycleTimeLevelUI, _lifeCycleLevel, _incCellLifeTimeLevel);
    }

    // =================================================
    // Onclick Decrease Self Divide Time
    // =================================================
    public void OnClickDecSelfDivideTime()
    {
        if (GameManager._GM.Point < _decSelfDivideTimeCost)
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
        divideTime = Mathf.Max(_minSelfDivideTime, divideTime - _decSelfDivideTime);
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
    public void OnClickIncDivideRate()
    {
        if (GameManager._GM.Point < _incDivideRateCost)
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
    // Onclick Increse Toxin Cost
    // =================================================
    public void OnClickIncToxinCost()
    {
        if (GameManager._GM.Point < _incToxinCostCost)
        {
            Debug.Log("Point 부족 → return");
            return;
        }

        if (_currentToxinCost >= _maxToxinCost)
        {
            Debug.Log("최대 가치 도달");
            return;
        }

        _currentToxinCost++;

        // 임시
        float cost = _incToxinCostCost;
        _incToxinCostLevel++;

        GameManager._GM.LoseDNAPoint(cost);
        UIManager._UM.CountUp(_incToxinCostUI, _toxinCostLevel, _incToxinCostLevel);
    }

    // =================================================
    // Onclick Increse Toxin Generate Rate
    // =================================================
    public void OnClickIncToxinGenRate()
    {
        if (GameManager._GM.Point < _incToxinGenRateCost)
        {
            Debug.Log("Point 부족 → return");
            return;
        }

        if(_currentToxinGenRate >= _maxToxinGenRate)
        {
            Debug.Log("최대 확률 도달");
            return;
        }

        _currentToxinGenRate = Mathf.Min(_maxToxinGenRate, _currentToxinGenRate + _incToxinGenRate);

        // 임시
        float cost = _incToxinGenRateCost;
        _toxinGenRateLevel++;

        GameManager._GM.LoseDNAPoint(cost);
        UIManager._UM.CountUp(_incToxinGenRateUI, _toxinGenRateLevel, _incToxinGenRateLevel);
    }

    // =================================================
    // SetUI
    // =================================================
    public void SetUI()
    {
        UIManager._UM.CurrentStat(_delayUI, _prodTUI, _amountUI, _lctUI, _sdtUI, _sdrUI, _tctUI, _tgrUI,
           _CSManager.ClickDelay, _currentProdTime, _CSManager.DivideAmount, _currentLifeTime,
           _CSManager.SelfDivideTime, _currentDivideRate, _currentToxinCost, _currentToxinGenRate);
    }

    // =================================================
    // Awake
    // =================================================
    void Awake()
    {
        _currentProdTime = _defaultProdTime;
        _currentLifeTime = _defaultCellLifeTime;
        _currentDivideRate = _defaultDivideRate;
        _currentToxinCost = _defaultToxinCost;
        _currentToxinGenRate = _defaultToxinGenRate;
    }

    // =================================================
    // Start
    // =================================================
    void Start()
    {
        _decDelay = (_defaultDelayTime - _minDelayTime) / _decDelayLevel;
        _incLifeCycleTime = (_maxCellLifeTime - _defaultCellLifeTime) / _incCellLifeTimeLevel;
        _decProdTime = (_defaultProdTime - _minProdTime) / _decProdLevel;
        _decSelfDivideTime = (_defaultSelfDivideTime - _minSelfDivideTime) / _decSelfdivideTimeLevel;
        _incSelfDivideRate = (_maxDivideRate - _defaultDivideRate) / _incSelfDivideRateLevel;
        _incToxinGenRate = (_maxToxinGenRate - _defaultToxinGenRate) / _incToxinGenRateLevel;
    }

    // =================================================
    // Update
    // =================================================
    void Update()
    {
        SetUI();
    }
}
