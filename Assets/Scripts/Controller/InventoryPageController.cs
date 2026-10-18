using UnityEngine;

public class InventoryPageController : MonoBehaviour
{
    // =================================================
    // Reference
    // =================================================
    [Header("Upgrade Pages")]
    [SerializeField] GameObject _status;
    [SerializeField] GameObject _upgrade1;
    [SerializeField] GameObject _upgrade2;
    [SerializeField] GameObject _tmp;

    public void OnClickShowStatus()
    {
        _status.SetActive(true);
        _upgrade1.SetActive(false);
        _upgrade2.SetActive(false);
        _tmp.SetActive(false);
    }

    public void OnClickShowUpgrade1()
    {
        _status.SetActive(false);
        _upgrade1.SetActive(true);
        _upgrade2.SetActive(false);
        _tmp.SetActive(false);
    }

    public void OnClickShowUpgrade2()
    {
        _status.SetActive(false);
        _upgrade1.SetActive(false);
        _upgrade2.SetActive(true);
        _tmp.SetActive(false);
    }

    public void OnClickShowTMP()
    {
        _status.SetActive(false);
        _upgrade1.SetActive(false);
        _upgrade2.SetActive(false);
        _tmp.SetActive(true);
    }

    // =================================================
    // Start
    // =================================================
    void Start()
    {
        OnClickShowStatus();
    }
}
