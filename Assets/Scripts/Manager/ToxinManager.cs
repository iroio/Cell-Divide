using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ToxinManager : MonoBehaviour
{
    // =================================================
    // Reference
    // =================================================
    [SerializeField] MouseController _musController;
    [SerializeField] GameObject _toxinPrefab;
    [SerializeField] Transform _toxinRoot;

    // =================================================
    // 마우스 정보
    // =================================================
    Collider2D _hit;

    // =================================================
    // Layer Hash
    // =================================================
    int _hash_toxinLayer;

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
    // CollectToxin
    // =================================================
    public bool CollectToxin()
    {
        _hit = Physics2D.OverlapPoint(_musController.MWorldPos, _hash_toxinLayer);
        
        if (_hit == null)
        {
            Debug.Log("클릭 위치에서 Toxin Collider를 찾지 못함");
            return false;
        }
        
        ToxinController toxin = _hit.GetComponent<ToxinController>();

        if(toxin == null)
        {
            Debug.Log($"{_hit.name}에서 ToxinController를 찾지 못함");
            return false;
        }

        GameManager._GM.GainDNAPoint();
        DeleteToxin(toxin);

        return true;
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

    // =================================================
    // Start
    // =================================================
    void Start()
    {
        _hash_toxinLayer = 1 << LayerMask.NameToLayer("Toxin");
    }

    // =================================================
    // Update
    // =================================================
    void Update()
    {
        if (!_musController.WasPressed()) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;

        CollectToxin();
    }
}
