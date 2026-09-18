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
