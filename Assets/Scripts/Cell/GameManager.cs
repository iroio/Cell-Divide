using UnityEngine;

public class GameManager : MonoBehaviour
{
    // =========================================================
    // Reference
    // =========================================================
    public static GameManager _GM;

    float _point = 0f;

    public float Point => _point;

    // =========================================================
    // StartGame
    // =========================================================
    public void StartGame()
    {
    }

    // =========================================================
    // GameOver
    // =========================================================
    public void GameOver()
    {
    }

    // =========================================================
    // ResetGame
    // =========================================================
    public void ResetGame()
    {
    }

    // =========================================================
    // GetPoint
    // =========================================================
    public void GainDNAPoint()
    {
        _point++;
        UIManager._UM.SetDNAPoint(_point);
        Debug.Log(_point);
    }
    public void LoseDNAPoint(float amount)
    {
        _point = Mathf.Max(0f, _point - amount);
        UIManager._UM.SetDNAPoint(_point);
        Debug.Log(_point);
    }

    private void Awake()
    {
        if (_GM == null)
        {
            _GM = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}