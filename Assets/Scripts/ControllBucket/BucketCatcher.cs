using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using VContainer;

public class BucketCatcher : MonoBehaviour
{
    [System.Serializable]
    public struct BucketVisualData
    {
        public FishType Type;
        public GameObject BucketObject;
    }

    [SerializeField] private Transform _visualTransform;

    [SerializeField] private BucketVisualData[] _buckets;
    [SerializeField] private float _defaultChangeInterval = 4f;

    [SerializeField] private ParticleSystem _catchParticles;

    [SerializeField] private Vector3 _punchScale = new Vector3(0.25f, -0.2f, 0f);
    [SerializeField] private float _punchDuration = 0.25f;
    [SerializeField] private int _vibrato = 8;
    [SerializeField] private float _elasticity = 0.5f;

    [Header("Fail Animation Settings")]
    [SerializeField] private float _shakeAngle = 18f; 
    [SerializeField] private float _shakeDuration = 0.3f;

    private ScoreModel _scoreModel;
    private AudioManager _audioManager;
    private FishType _currentBucketType = FishType.Blue;
    private float _actualChangeInterval;
    private bool _isGameActive = true;

    private Vector3 _initialScale;

    [Inject]
    public void Construct(ScoreModel scoreModel, AudioManager audioManager)
    {
        _scoreModel = scoreModel;
        _audioManager = audioManager;
    }

    private void Start()
    {
        if (_visualTransform == null)
            _visualTransform = transform;

        _initialScale = _visualTransform.localScale;

        _actualChangeInterval = GameSettings.SelectedDifficulty != null
            ? GameSettings.SelectedDifficulty.bucketChangeInterval
            : _defaultChangeInterval;

        UpdateActiveBucket();
        ChangeColorLoopAsync(this.GetCancellationTokenOnDestroy()).Forget();
    }

    public void StopCatching()
    {
        _isGameActive = false;
        enabled = false;
    }

    public void ResetCatcher()
    {
        _isGameActive = true;
        enabled = true;
        UpdateActiveBucket();
    }

    private async UniTaskVoid ChangeColorLoopAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            await UniTask.Delay((int)(_actualChangeInterval * 1000), cancellationToken: ct);

            if (!_isGameActive) continue;

            SetRandomBucketType();
        }
    }

    private void SetRandomBucketType()
    {
        if (_buckets == null || _buckets.Length == 0) return;

        int randomIndex;
        do
        {
            randomIndex = UnityEngine.Random.Range(0, _buckets.Length);
        }
        while (_buckets.Length > 1 && _buckets[randomIndex].Type == _currentBucketType);

        _currentBucketType = _buckets[randomIndex].Type;
        UpdateActiveBucket();
    }

    private void UpdateActiveBucket()
    {
        foreach (var bucketData in _buckets)
        {
            if (bucketData.BucketObject != null)
            {
                bool isActive = (bucketData.Type == _currentBucketType);
                bucketData.BucketObject.SetActive(isActive);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_isGameActive) return;

        if (collision.TryGetComponent(out FishMovement fish))
        {
            if (fish.Data != null)
            {
                int multiplier = GameSettings.SelectedDifficulty != null
                    ? GameSettings.SelectedDifficulty.scoreMultiplier
                    : 1;

                int calculatedPoints = fish.Data.ScoreValue * multiplier;

                if (fish.Data.Type == _currentBucketType)
                {
                    _scoreModel.AddScore(calculatedPoints);
                    PlaySuccessAnimation();
                }
                else
                {
                    _scoreModel.AddScore(-calculatedPoints);
                    PlayFailAnimation();
                }
            }
            fish.Despawn();
        }
    }

    private void PlaySuccessAnimation()
    {
        Transform target = _visualTransform != null ? _visualTransform : transform;

        target.DOKill(true);
        target.localScale = _initialScale;

        target.DOPunchScale(_punchScale, _punchDuration, _vibrato, _elasticity);

        if (_catchParticles != null)
        {
            _catchParticles.Stop();
            _catchParticles.Play();
        }

        _audioManager?.PlayCatchSuccess();
    }

    private void PlayFailAnimation()
    {
        Transform target = _visualTransform != null ? _visualTransform : transform;

        target.DOKill(true);
        target.localRotation = Quaternion.identity;

        target.DOPunchRotation(new Vector3(0f, 0f, _shakeAngle), _shakeDuration, 10, 0.5f)
              .OnComplete(() => target.localRotation = Quaternion.identity);

        _audioManager?.PlayCatchFail();
    }

    private void OnDestroy()
    {
        if (_visualTransform != null) _visualTransform.DOKill();
        transform.DOKill();
    }
}