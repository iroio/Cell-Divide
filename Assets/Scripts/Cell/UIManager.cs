using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // =========================================================
    // Reference
    // =========================================================
    [SerializeField] TextMeshProUGUI _dnaPointUI;

    public static UIManager _UM;

    public void SetDNAPoint(float point)
    {
        _dnaPointUI.text = point.ToString();
    }

    public void CountUp(TextMeshProUGUI levelUI, int level)
    {
        levelUI.text = "+" + level.ToString();
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
