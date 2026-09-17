using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

public class WaterFall : MonoBehaviour
{
    [SerializeField] private ParticleSystem _splashWaterPrefab;

    [Header("Settings pool particles")]
    [SerializeField] private int _defaultCapacity = 10;
    [SerializeField] private int _maxSize = 20;

    private IObjectPool<ParticleSystem> _splashPool;

    private void Awake()
    {
        if (_splashWaterPrefab == null) return;

        _splashPool = new ObjectPool<ParticleSystem>(
            createFunc: () => Instantiate(_splashWaterPrefab, transform),
            actionOnGet: splash => splash.gameObject.SetActive(true),
            actionOnRelease: splash => splash.gameObject.SetActive(false),
            actionOnDestroy: splash => Destroy(splash.gameObject),
            collectionCheck: false,
            defaultCapacity: _defaultCapacity,
            maxSize: _maxSize
        );
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out FishMovement fish))
        {
            fish.StopMoving();
            fish.Die();
        }

        SpawnSplashEffect(collision.transform.position);
    }

    private void SpawnSplashEffect(Vector3 position)
    {
        if (_splashPool == null) return;

        ParticleSystem splash = _splashPool.Get();
        splash.transform.position = position;
        splash.Play();

        ReturnToPoolAsync(splash).Forget();
    }

    private async UniTaskVoid ReturnToPoolAsync(ParticleSystem splash)
    {
        var ct = this.GetCancellationTokenOnDestroy();

        await UniTask.WaitUntil(() => !splash.IsAlive(true), cancellationToken: ct);

        if (splash != null)
        {
            _splashPool.Release(splash);
        }
    }
}