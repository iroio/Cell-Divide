using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // =========================================================
    // Reference
    // =========================================================
    [SerializeField] TextMeshProUGUI _dnaPointUI;

    public static UIManager _UM;

    // =========================================================
    // Set DNA Point
    // =========================================================
    public void SetDNAPoint(float point)
    {
        _dnaPointUI.text = point.ToString();
    }

    // =========================================================
    // Purchase
    // =========================================================
    public void Purchase(TextMeshProUGUI levelUI, bool isBuy)
    {
        if(isBuy == true)
            levelUI.text = "Purchase";
    }

    // =========================================================
    // CountUp
    // =========================================================
    public void CountUp(TextMeshProUGUI levelUI, int level, int maxLevel)
    {
        int curLev = level;
        int maxLev = maxLevel;

        if (curLev >= maxLev)
        { 
            levelUI.text = "MAX LEVEL";
        }
        else
        {
            levelUI.text = "+" + level.ToString();
        }
    }

    // =========================================================
    // Cell CountUp
    // =========================================================
    public void CellCountUp(TextMeshProUGUI countUi, int curCount)
    {
        countUi.text = curCount.ToString();
    }

    // =========================================================
    // CurrentStat
    // =========================================================
    public void CurrentStat(TextMeshProUGUI delayUi, TextMeshProUGUI prodTUi,
        TextMeshProUGUI amountUi, TextMeshProUGUI lctUi, TextMeshProUGUI sdtUi,
        float delay, float prodT, int amount, float lct, float sdt)
    {
        delayUi.text = delay.ToString("0.###");
        prodTUi.text  = prodT.ToString("0.###");
        amountUi.text = amount.ToString();
        lctUi.text = lct.ToString("0.###");
        sdtUi.text = sdt.ToString("0.###");
    }

    // =========================================================
    // Awake
    // =========================================================
    public void Awake()
    {
        _UM = this;
    }

    // =========================================================
    // Start
    // =========================================================
    void Start()
    {

    }

    // =========================================================
    // Update
    // =========================================================
    void Update()
    {
        
    }
}
