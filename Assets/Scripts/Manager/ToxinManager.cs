using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToxinManager : MonoBehaviour
{
    // =================================================
    // Reference
    // =================================================
    [SerializeField] MouseManager _musManager;
    [SerializeField] ParticleManager _ptcleManager;

    [SerializeField] GameObject toxinPrefab;
    [SerializeField] Transform _toxinRoot;

    // =================================================
    // 마우스 정보
    // =================================================
    Collider2D _hit;

    // =================================================
    // 파티클 위치
    // =================================================
    Vector2 _toxinPos;

    // =================================================
    // Layer Hash
    // =================================================
    int _hash_toxinLayer;

    // =================================================
    // Object Pool
    // =================================================
    GameObjectPool<ToxinController> _toxinControllerPool;

    // =================================================
    // SpawnToxin
    // =================================================
    public void SpawnToxin(CellController parentCell)
    {
        var toxin = _toxinControllerPool.Get();

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
        _hit = Physics2D.OverlapPoint(_musManager.MWorldPos, _hash_toxinLayer);
        if (_hit == null) return false;
        
        ToxinController toxin = _hit.GetComponent<ToxinController>();
        if(toxin == null) return false;

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

        _toxinPos = toxin.transform.position;

        // Toxin Set()
        toxin.gameObject.SetActive(false);
        _toxinControllerPool.Set(toxin);

        // 삭제시 파티클 실행
        var toxicle = _ptcleManager.TXPool.Get();

        if (toxicle == null) return;

        toxicle.transform.position = _toxinPos;
        toxicle.gameObject.SetActive(true);
        toxicle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        toxicle.Play();

        _ptcleManager.StartCoReturnParticle(toxicle);
    }

    // =================================================
    // Awake
    // =================================================
    void Awake()
    {
        _toxinControllerPool = new GameObjectPool<ToxinController>(5, () =>
        {
            var obj = Instantiate(toxinPrefab, _toxinRoot);
            obj.SetActive(false);

            return obj.GetComponent<ToxinController>();
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
        if (!_musManager.WasPressed()) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;

        CollectToxin();
    }
}
