using TMPro;
using UnityEngine;

public class UpgradesManager : MonoBehaviour
{
    // =================================================
    // Reference
    // =================================================
    [SerializeField] CellSpawnManager _CSManager;
    [SerializeField] TextMeshProUGUI _reduceDelayLevelUI;
    [SerializeField] TextMeshProUGUI _increseDivideAmountLevelUI;

    // =================================================
    // Delay Option
    // =================================================
    [SerializeField] float _maxTime = 10f;
    [SerializeField] float _minTime = 0.2f;
    [SerializeField] int _reduceLevel = 50;

    int _reduceDelayLevel = 0;
    int _increseDivideAmountLevel = 0;
    float _decrease;

    // =================================================
    // Divide Option
    // =================================================

    // =================================================
    // OnclickReduceDelay
    // =================================================
    public void OnclickReduceDelay()
    {
        if (GameManager._GM.Point < 1f)
        {
            Debug.Log("Point 부족 → return");
            return;
        }

        if (_CSManager.ClickDelay <= _minTime)
        {
            Debug.Log("최소 딜레이 도달 → return");
            return;
        }

        float delay = _CSManager.ClickDelay;

        delay = Mathf.Max(_minTime, delay - _decrease);

        _CSManager.ClickDelayReduce(delay);

        // 임시
        float amount = 1f;
        _reduceDelayLevel++;

        GameManager._GM.LoseDNAPoint(amount);
        UIManager._UM.CountUp(_reduceDelayLevelUI, _reduceDelayLevel);
    }

    // =================================================
    // OnclickReduceDelay
    // =================================================
    public void OnclickIncreseDivideAmount()
    {
        if (GameManager._GM.Point < 1f)
        {
            Debug.Log("Point 부족 → return");
            return;
        }

        int divideAmount = _CSManager.DivideAmount;
        divideAmount++;
        _CSManager.IncreseDivideAmount(divideAmount);

        //임시
        float amount = 1f;
        _increseDivideAmountLevel++;

        GameManager._GM.LoseDNAPoint(amount);
        UIManager._UM.CountUp(_increseDivideAmountLevelUI, _increseDivideAmountLevel);
    }


    // =================================================
    // Option
    // =================================================
    void Start()
    {
        _decrease = (_maxTime - _minTime) / _reduceLevel;
    }

    // =================================================
    // Option
    // =================================================
    void Update()
    {
        
    }
}
