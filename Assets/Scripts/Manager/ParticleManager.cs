using UnityEngine;
using System.Collections;

public class ParticleManager : MonoBehaviour
{
    // =================================================
    // Reference
    // =================================================
    [SerializeField] GameObject _particlePrefab;
    [SerializeField] Transform _particleRoot;

    [SerializeField] GameObject _toxiclePrefab;
    [SerializeField] Transform _toxicleRoot;

    // =================================================
    // Object Pool
    // =================================================
    GameObjectPool<ParticleSystem> _psPool;
    GameObjectPool<ParticleSystem> _txPool;

    public GameObjectPool<ParticleSystem> PSPool => _psPool;
    public GameObjectPool<ParticleSystem> TXPool => _txPool;

    // =================================================
    // CoReturnParticle
    // =================================================
    IEnumerator CoReturnParticle(ParticleSystem prticle)
    {
        yield return new WaitUntil(() => !prticle.IsAlive());

        prticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        prticle.gameObject.SetActive(false);

        if (prticle.CompareTag("Particle"))
            PSPool.Set(prticle);
        else if(prticle.CompareTag("Toxicle"))
            TXPool.Set(prticle);
    }

    public void StartCoReturnParticle(ParticleSystem prticle)
    {
        StartCoroutine(CoReturnParticle(prticle));
    }

    // =================================================
    // Awake
    // =================================================
    void Awake()
    {
        _psPool = new GameObjectPool<ParticleSystem>(5, () =>
        {
            var obj = Instantiate(_particlePrefab, _particleRoot);
            obj.SetActive(false);

            return obj.GetComponent<ParticleSystem>();
        });

        _txPool = new GameObjectPool<ParticleSystem>(5, () =>
        {
            var obj = Instantiate(_toxiclePrefab, _toxicleRoot);
            obj.SetActive(false);

            return obj.GetComponent<ParticleSystem>();
        });
    }
}
