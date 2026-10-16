using UnityEngine;

public class ToxinManager : MonoBehaviour
{
    // =================================================
    // Reference
    // =================================================
    [SerializeField] GameObject _toxinPrefab;
    [SerializeField] Transform _toxinRoot;

    // =================================================
    // Object Pool
    // =================================================
    GameObjectPool<ToxinController> _toxinPool;

    // =================================================
    // SpawnToxin
    // =================================================
    public void SpawnToxin(CellController parentCell)
    {
        var toxin = _toxinPool.Get();

        if(toxin == null)
        {
            Debug.Log("Pool에서 toxin을 가져오지 못함");
            return;
        }

        toxin.transform.position = parentCell.transform.position;
        toxin.gameObject.SetActive(true);

        toxin.StartToxinLifeCycle();
    }

    // =================================================
    // Delete Toxin
    // =================================================
    public void DeleteToxin(ToxinController toxin)
    {
        if (!toxin.gameObject.activeSelf) return;

        toxin.gameObject.SetActive(false);
        _toxinPool.Set(toxin);
    }

    // =================================================
    // Awake
    // =================================================
    void Awake()
    {
        _toxinPool = new GameObjectPool<ToxinController>(10, () =>
        {
            var obj = Instantiate(_toxinPrefab, _toxinRoot);
            obj.SetActive(false);
            var toxin = obj.GetComponent<ToxinController>();
            toxin.InitToxin(this);

            return toxin;
        });
    }
}
