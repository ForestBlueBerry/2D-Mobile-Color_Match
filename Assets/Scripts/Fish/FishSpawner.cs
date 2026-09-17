using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;

public class FishSpawner : MonoBehaviour
{
    [SerializeField] private FishSO[] _fishConfigs;
    [SerializeField] private BoxCollider2D _spawnArea;
    [SerializeField] private float _baseSpawnInterval = 2f;
    [SerializeField] private int _defaultCapacity = 10;
    [SerializeField] private int _maxSize = 30;

    private bool _isSpawning = true;
    private Dictionary<FishSO, IObjectPool<FishMovement>> _pools;
    private DifficultyConfig _currentDifficulty;

    private AudioManager _audioManager;

    [Inject]
    public void Construct(AudioManager audioManager)
    {
        _audioManager = audioManager;
    }

    private void Awake()
    {
        InitPools();
    }

    private void Start()
    {
        _currentDifficulty = GameSettings.SelectedDifficulty;
        SpawnLoopAsync(this.GetCancellationTokenOnDestroy()).Forget();
    }

    private void InitPools()
    {
        _pools = new Dictionary<FishSO, IObjectPool<FishMovement>>();

        foreach (var config in _fishConfigs)
        {
            if (config == null || config.Prefab == null) continue;

            var pool = new ObjectPool<FishMovement>(
                createFunc: () => Instantiate(config.Prefab, transform),
                actionOnGet: fish => fish.gameObject.SetActive(true),
                actionOnRelease: fish => fish.gameObject.SetActive(false),
                actionOnDestroy: fish => Destroy(fish.gameObject),
                collectionCheck: false,
                defaultCapacity: _defaultCapacity,
                maxSize: _maxSize
            );

            _pools.Add(config, pool);
        }
    }

    private async UniTaskVoid SpawnLoopAsync(CancellationToken ct)
    {
        float speedMultiplier = (_currentDifficulty != null) ? _currentDifficulty.gameSpeed : 1f;
        float actualInterval = Mathf.Max(0.25f, _baseSpawnInterval / speedMultiplier);

        while (_isSpawning && !ct.IsCancellationRequested)
        {
            SpawnRandomFish();
            await UniTask.Delay((int)(actualInterval * 1000), cancellationToken: ct);
        }
    }

    private void SpawnRandomFish()
    {
        if (_fishConfigs == null || _fishConfigs.Length == 0 || _spawnArea == null)
            return;

        FishSO selectedConfig = _fishConfigs[Random.Range(0, _fishConfigs.Length)];

        if (!_pools.TryGetValue(selectedConfig, out var pool)) return;

        Vector3 spawnPosition = GetRandomPointInBounds(_spawnArea.bounds);

        FishMovement fishInstance = pool.Get();
        fishInstance.transform.position = spawnPosition;
        fishInstance.transform.rotation = Quaternion.identity;

        fishInstance.Initialize(selectedConfig, fish => pool.Release(fish), _audioManager);
    }

    private Vector3 GetRandomPointInBounds(Bounds bounds)
    {
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomY = Random.Range(bounds.min.y, bounds.max.y);

        return new Vector3(randomX, randomY, transform.position.z);
    }

    public void StopSpawning() => _isSpawning = false;
}