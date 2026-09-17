using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class FishMovement : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private float _waterfallY = 3.5f;
    [SerializeField] private AudioClip _audioClip;

    private AudioManager _audioManager;
    private float _swimSpeed;
    private float _fallSpeed;
    private bool _isFalling;
    private bool _isStopped;

    private Action<FishMovement> _onReleaseToPool;

    public FishSO Data { get; private set; }

    private void Awake()
    {
        if (_animator == null) TryGetComponent(out _animator);
    }

    public void Initialize(FishSO data, Action<FishMovement> onReleaseToPool, AudioManager audioManager)
    {
        Data = data;
        _audioManager = audioManager;

        float speedMultiplier = GameSettings.SelectedDifficulty != null ? GameSettings.SelectedDifficulty.gameSpeed : 1.0f;
        _swimSpeed = data.SwimSpeed * speedMultiplier;
        _fallSpeed = data.FallSpeed * speedMultiplier;

        _onReleaseToPool = onReleaseToPool;

        _isFalling = false;
        _isStopped = false;

        if (_animator != null)
        {
            _animator.Rebind();
            _animator.ResetTrigger("IsDie");
            _animator.SetBool("IsFall", false);
        }

        if (TryGetComponent(out SpriteRenderer sr))
        {
            sr.flipX = UnityEngine.Random.value > 0.5f;
        }
    }

    private void Update()
    {
        if (_isStopped) return;

        if (!_isFalling)
        {
            transform.Translate(Vector3.down * (_swimSpeed * Time.deltaTime), Space.World);
            if (transform.position.y <= _waterfallY)
            {
                _isFalling = true;

                if (_animator != null)
                    _animator.SetBool("IsFall", true);
            }
        }
        else
        {
            transform.Translate(Vector3.down * (_fallSpeed * Time.deltaTime), Space.World);
        }
    }

    public void StopMoving() => _isStopped = true;

    public void Die()
    {
        _isStopped = true;

        if (_audioClip != null && _audioManager != null)
        {
            _audioManager.PlaySFX(_audioClip);
        }

        if (_animator != null)
        {
            _animator.SetTrigger("IsDie");
        }

        DelayedDespawnAsync().Forget();
    }

    private async UniTaskVoid DelayedDespawnAsync()
    {
        await UniTask.Delay(1000, cancellationToken: this.GetCancellationTokenOnDestroy());
        Despawn();
    }

    public void Despawn()
    {
        _onReleaseToPool?.Invoke(this);
    }
}