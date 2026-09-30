using TMPro;
using UnityEngine;

public class CellMediaManager : MonoBehaviour
{
    // =========================================================
    // Reference
    // =========================================================
    [SerializeField] Transform _cellRoot;

    [SerializeField] TextMeshProUGUI _cellCount;

    int _childCount = 0;

    // =========================================================
    // Property
    // =========================================================
    public int ChildCount => _childCount;

    // =========================================================
    // ChilldCount
    // =========================================================
    public void ChilldCount()
    {
        _childCount = 0;

        int count = _cellRoot.transform.childCount;

        for (int i = 0; i < count; i++)
        {
            Transform child = _cellRoot.transform.GetChild(i);

            if(child.gameObject.activeSelf)
            {
                _childCount++;
            }
        }

        UIManager._UM.CellCountUp(_cellCount, _childCount);
    }
}
