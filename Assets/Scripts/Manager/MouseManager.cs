using UnityEngine;
using UnityEngine.InputSystem;

public class MouseManager : MonoBehaviour
{
    // =================================================
    // Reference
    // =================================================
    [SerializeField] InputActionReference _divide;

    Vector2 _mPos;
    Vector2 _mWorldPos = new Vector2(-11f, 0f);

    public Vector2 MWorldPos => _mWorldPos;

    // =================================================
    // Mouse Position
    // =================================================
    public void CalcMousePos()
    {
        _mPos = Mouse.current.position.ReadValue();
        _mWorldPos = Camera.main.ScreenToWorldPoint(_mPos);
    }

    // =================================================
    // Mouse Input
    // =================================================
    public bool WasPressed()
    {
        return _divide.action.WasPressedThisFrame();
    }

    // =================================================
    // Input System Enable
    // =================================================
    void OnEnable()
    {
        if (_divide != null && _divide.action != null)
        {
            _divide.action.Enable();
        }
    }

    // =================================================
    // Input System Disable
    // =================================================
    void OnDisable()
    {
        if (_divide != null && _divide.action != null)
        {
            _divide.action.Disable();
        }
    }

    // =================================================
    // Update
    // =================================================
    // Update is called once per frame
    void Update()
    {
        CalcMousePos();
    }
}
